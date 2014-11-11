using CapnProto.Schema;
using CapnProto.Take2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
namespace CapnProto
{
    public class CSharpStructWriter : CodeWriter
    {
        public override bool NeedsSerializer
        {
            get { return false; }
        }
        public CSharpStructWriter(TextWriter destination, List<Schema.Node> nodes,
            string @namespace, string serializer)
            : base(destination, nodes, @namespace, serializer)
        { }
        private int indentationLevel;

        public override CodeWriter WriteLine(bool indent = true)
        {
            base.WriteLine();
            if (indent)
            {
                int tmp = indentationLevel;
                while (tmp-- > 0)
                {
                    Write("    ");
                }
            }
            return this;
        }
        public override CodeWriter WriteSerializerTest(string field, Node node, string serializer)
        {
            throw new NotSupportedException();
        }
        public override CodeWriter WriteCustomReaderMethod(Node node)
        {
            throw new NotSupportedException();
        }
        static string Escape(string name)
        {
            if (name == null) return null;
            switch (name)
            {
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "virtual":
                case "void":
                case "volatile":
                case "while":
                    return "@" + name;
                default:
                    return name;
            }
        }

        public override CodeWriter Write(bool value)
        {
            return Write(value ? "true" : "false");
        }
        public override CodeWriter WriteLiteral(string value)
        {
            if (value == null) return Write("null");
            return Write("@\"").Write(value.Replace("\"", "\"\"")).Write("\"");
        }
        public override CodeWriter WriteConst(Schema.Node node)
        {
            if (node == null || node.Union != Schema.Node.Unions.@const) return this;
            var @const = node.@const;
            return WriteLine().Write("public const ").Write(@const.type).Write(" ").Write(LocalName(node)).Write(" = ").Write(@const.type, @const.value).Write(";");
        }
        public override CodeWriter BeginClass(Schema.Node node)
        {
            if (node.Union == Schema.Node.Unions.@struct)
            {
                var @struct = node.@struct;
                if (@struct.isGroup.Value)
                {
                    WriteLine().Write("[").Write(typeof(GroupAttribute)).Write("]");
                }
                else
                {
                    WriteLine().Write("[").Write(typeof(StructAttribute)).Write("(").Write(typeof(ElementSize)).Write(".")
                        .Write(((ElementSize)@struct.preferredListEncoding.Value).ToString()).Write(", ")
                        .Write(@struct.dataWordCount.Value).Write(", ").Write(@struct.pointerCount).Write(")]");
                }
                if (node.id != 0)
                {
                    WriteLine().Write("[").Write(typeof(IdAttribute)).Write("(").Write(node.id).Write(")]");
                }
            }
            string localName = LocalName(node), fullName = FullyQualifiedName(node);
            WriteLine().Write("public struct ").Write(localName);
            Indent();
            WriteLine().Write("private readonly ").Write(typeof(Pointer)).Write(" ").Write(PointerName).Write(";");
            WriteLine().Write("private ").Write(localName).Write("(").Write(typeof(Pointer)).Write(" pointer){ this.").Write(PointerName).Write(" = pointer; }");
            WriteLine().Write("public static explicit operator ").Write(fullName).Write("(").Write(typeof(Pointer)).Write(" pointer) { return new ").Write(fullName).Write("(pointer); }");
            WriteLine().Write("public static implicit operator ").Write(typeof(Pointer)).Write(" (").Write(fullName).Write(" obj) { return obj.").Write(PointerName).Write("; }");
            WriteLine().Write("public static bool operator true(").Write(fullName).Write(" obj) { return obj.").Write(PointerName).Write(".IsValid; }");
            WriteLine().Write("public static bool operator false(").Write(fullName).Write(" obj) { return !obj.").Write(PointerName).Write(".IsValid; }");
            WriteLine().Write("public static bool operator !(").Write(fullName).Write(" obj) { return !obj.").Write(PointerName).Write(".IsValid; }");
            WriteLine().Write("public override int GetHashCode() { return this.").Write(PointerName).Write(".GetHashCode(); }");
            WriteLine().Write("public override string ToString() { return this.").Write(PointerName).Write(".ToString(); }");
            WriteLine().Write("public override bool Equals(object obj) { return obj is ").Write(fullName).Write(" && (this.")
                .Write(PointerName).Write(" == ((").Write(fullName).Write(")obj).").Write(PointerName).Write("); }");
            WriteLine().Write("public ").Write(fullName).Write(" Dereference() { return (").Write(fullName).Write(")this.").Write(PointerName).Write(".Dereference(); }");
            if (node.Union == Node.Unions.@struct)
            {
                if (node.@struct.discriminantCount != 0)
                {
                    WriteLine().Write("public static ").Write(fullName).Write(" Create(").Write(typeof(Pointer)).Write(" parent, ").Write(fullName).Write(".Unions union)");
                    Indent();
                    WriteLine().Write("var ptr = parent.Allocate(").Write(node.@struct.dataWordCount).Write(", ").Write(node.@struct.pointerCount).Write(");");
                    WriteLine().Write("ptr.SetUInt16(").Write(node.@struct.discriminantOffset).Write(", (ushort)union);");
                    WriteLine().Write("return (").Write(fullName).Write(")ptr;");
                    Outdent();
                }
                else
                {
                    WriteLine().Write("public static ").Write(fullName).Write(" Create(").Write(typeof(Pointer)).Write(" parent) { return (").Write(fullName).Write(")parent.Allocate(")
                        .Write(node.@struct.dataWordCount).Write(", ").Write(node.@struct.pointerCount).Write("); }");
                }
            }
            return this;
        }
        private static Stack<UnionStub> Clone(Stack<UnionStub> union)
        {
            if (union == null) return null;
            if (union.Count == 0) return union; // safe when empty
            var tmp = new Stack<UnionStub>(union.Count);
            foreach (var item in union)
                tmp.Push(item);
            return tmp;
        }
        

        void WriteListImpl(Schema.Field field)
        {
            var elType = field.slot.type.list.elementType;
            int index = (int)field.slot.offset;

            switch (elType.Union)
            {
                case Schema.Type.Unions.text:
                    Write("ctx.Reader.ReadStringList(segment, origin + ").Write(index + 1).Write(", raw[").Write(index).Write("]);");
                    break;
                case Schema.Type.Unions.@struct:
                    var found = Lookup(elType.@struct.typeId);
                    if (found == null)
                    {
                        Write("null; #error type not found: ").Write(elType.@struct.typeId);
                    }
                    else if (found.Union != Schema.Node.Unions.@struct || found.IsGroup())
                    {
                        Write("null; #error invalid type for list: ").Write(found.displayName);
                    }
                    else
                    {
                        Write("ctx.Reader.ReadStructList<").Write(FullyQualifiedName(found)).Write(">(ctx, segment, origin + ").Write(index + 1).Write(", raw[").Write(index).Write("]);");
                    }
                    break;
                case Schema.Type.Unions.int64:
                    Write("ctx.Reader.ReadInt64List(segment, origin, raw[").Write(index).Write("]);");
                    break;
                default:
                    Write("null;").WriteLine().Write("#warning not yet supported: " + elType.Union);
                    break;
            }
        }

        private static void CascadePointers(CodeWriter writer, Schema.Node node, SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>> ptrFields, Stack<UnionStub> union)
        {
            if (node.@struct.fields != null)
            {
                foreach (var field in node.@struct.fields)
                {
                    bool isFiltered = field.discriminantValue != Field.noDiscriminant;
                    switch (field.Union)
                    {
                        case Field.Unions.group:
                            {
                                var found = writer.Lookup(field.group.typeId);
                                if (found != null)
                                {
                                    if (isFiltered) union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                                    CascadePointers(writer, found, ptrFields, union);
                                    if (isFiltered) union.Pop();
                                }
                                break;
                            }
                        case Field.Unions.slot:
                            {
                                var slot = field.slot;
                                if (slot.type == null) continue;
                                int len = slot.type.GetFieldLength();

                                if (len == Schema.Type.LEN_POINTER)
                                {
                                    if (isFiltered) union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                                    List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>> fields;
                                    if (!ptrFields.TryGetValue((int)slot.offset, out fields))
                                    {
                                        ptrFields.Add((int)slot.offset, fields = new List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>());
                                    }
                                    fields.Add(Tuple.Create(node, field, Clone(union)));
                                    if (slot.type.Union == Schema.Type.Unions.@struct)
                                    {
                                        var found = writer.Lookup(slot.type.@struct.typeId);
                                        if (found != null && found.IsGroup())
                                        {
                                            CascadePointers(writer, found, ptrFields, union);
                                        }
                                    }
                                    if (isFiltered) union.Pop();
                                }
                                break;
                            }
                    }
                }
            }
        }


        static readonly char[] period = { '.' };
        public override CodeWriter BeginNamespace(string name)
        {
            WriteLine().Write("namespace ");
            var parts = name.Split(period);
            Write(Escape(parts[0]));
            for (int i = 1; i < parts.Length; i++)
            {
                Write(".").Write(Escape(parts[i]));
            }
            return Indent(); ;

        }

        public override CodeWriter BeginClass(bool @public, bool @internal, string name, System.Type baseType)
        {
            WriteLine();
            if (@public) Write("public ");
            else if (@internal) Write("internal ");
            Write("class ").Write(Escape(name));
            if (baseType != null)
            {
                Write(" : ").Write(baseType);
            }
            return Indent();
        }
        private CodeWriter Indent()
        {
            var tmp = WriteLine().Write("{");
            indentationLevel++;
            return tmp;
        }
        public override CodeWriter EndNamespace()
        {
            return Outdent();
        }
        private CodeWriter Outdent()
        {
            indentationLevel--;
            return WriteLine().Write("}");
        }
        public override CodeWriter EndClass()
        {
            return Outdent();
        }

        public override CodeWriter DeclareField(string name, System.Type type)
        {
            return WriteLine().Write("private ").Write(type).Write(" ").Write(Escape(name)).Write(";");
        }
        public override CodeWriter DeclareFields(string prefix, int count, System.Type type)
        {
            if (count != 0)
            {
                WriteLine().Write("private ").Write(type).Write(" ").Write(prefix).Write(0);
                for (int i = 1; i < count; i++)
                {
                    Write(", ").Write(prefix).Write(i);
                }
                Write(";");
            }
            return this;
        }
        public override CodeWriter DeclareFields(List<string> names, System.Type type)
        {
            if (names.Count != 0)
            {
                WriteLine().Write("private ").Write(type).Write(" ").Write(names[0]);
                for (int i = 1; i < names.Count; i++)
                {
                    Write(", ").Write(names[i]);
                }
                Write(";");
            }
            return this;
        }


        public override CodeWriter BeginOverride(System.Reflection.MethodInfo method)
        {
            WriteLine();
            if (method.IsPublic) Write("public override ");
            else if (method.IsFamilyOrAssembly) Write("protected internal override ");
            else if (method.IsFamily) Write("protected override ");
            else if (method.IsAssembly) Write("internal override ");
            Write(method.ReturnType).Write(" ").Write(method.Name).Write("(");
            var args = method.GetParameters();
            for (int i = 0; i < args.Length; i++)
            {
                if (i != 0) Write(", ");
                Write(args[0].ParameterType).Write(" ").Write(Escape(args[0].Name));
            }
            Write(")");
            return Indent();


        }

        public override CodeWriter CallBase(System.Reflection.MethodInfo method)
        {
            WriteLine();
            if (method.ReturnType != null && method.ReturnType != typeof(void)) Write("return ");
            Write("base.").Write(method.Name).Write("(");
            var args = method.GetParameters();
            for (int i = 0; i < args.Length; i++)
            {
                if (i != 0) Write(", ");
                Write(Escape(args[0].Name));
            }
            return Write(");");
        }

        public override CodeWriter EndOverride()
        {
            return Outdent();
        }

        public override CodeWriter WriteError(string message)
        {
            return WriteLine().Write("#error ").Write(message);
        }

        public override CodeWriter WriteWarning(string message)
        {
            return WriteLine().Write("#warning ").Write(message);
        }

        public override CodeWriter Write(System.Type type)
        {
            return Write(Format(type));
        }
        private string Format(System.Type type)
        {
            if (type == null || type == typeof(void)) return "void";
            if (!type.IsEnum) // reports same typecodes
            {
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean: return "bool";
                    case TypeCode.Byte: return "byte";
                    case TypeCode.Char: return "char";
                    case TypeCode.Double: return "double";
                    case TypeCode.Int16: return "short";
                    case TypeCode.Int32: return "int";
                    case TypeCode.Int64: return "long";
                    case TypeCode.SByte: return "sbyte";
                    case TypeCode.Single: return "float";
                    case TypeCode.String: return "string";
                    case TypeCode.UInt16: return "ushort";
                    case TypeCode.UInt32: return "uint";
                    case TypeCode.UInt64: return "ulong";
                }
            }
            if (type == typeof(object)) return "object"; // cant use TypeCode.Object: most classes etc report thats
            return "global::" + type.FullName.Replace('+', '.');
        }

        public override CodeWriter WriteCustomSerializerClass(Schema.Node node, string typeName, string methodName)
        {
            WriteLine().Write("class ").Write(typeName).Write(" : global::").Write(Namespace).Write(".").Write(Escape(Serializer))
                .Write(".").Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(", global::CapnProto.ITypeSerializer<").Write(FullyQualifiedName(node)).Write(">");
            Indent();

            WriteLine().Write("private readonly ").Write(typeof(TypeModel)).Write(" model;");
            WriteLine().Write("internal ").Write(typeName).Write("(").Write(typeof(TypeModel)).Write(" model)");
            Indent();
            WriteLine().Write("this.model = model;");
            Outdent();

            WriteLine().Write(typeof(TypeModel)).Write(" ").Write(typeof(ITypeSerializer)).Write(".Model { get { return this.model; } }");

            WriteLine().Write("object ").Write(typeof(ITypeSerializer)).Write(".Deserialize(int segment, int origin, ").Write(typeof(DeserializationContext)).Write(" ctx, ulong pointer)");
            Indent().WriteLine().Write("return ").Write(methodName).Write("(segment, origin, ctx, pointer);");
            EndMethod();

            WriteLine().Write("public ").Write(FullyQualifiedName(node)).Write(" Deserialize(int segment, int origin, ").Write(typeof(DeserializationContext)).Write(" ctx, ulong pointer)");
            Indent().WriteLine().Write("return ").Write(methodName).Write("(segment, origin, ctx, pointer);");
            EndMethod();

            return EndClass();
        }


        public override CodeWriter EndMethod()
        {
            return Outdent();
        }

        private string FullyQualifiedName(Schema.Node node)
        {
            var parent = FindParent(node);
            Stack<Schema.Node> roots = null;
            if (parent != null && parent.file == null)
            {
                roots = new Stack<Schema.Node>();
                do
                {
                    roots.Push(parent);
                    parent = FindParent(parent);
                } while (parent != null && parent.file == null);
            }
            var sb = new StringBuilder("global::");
            if (!string.IsNullOrWhiteSpace(Namespace)) sb.Append(Namespace).Append('.');
            while (roots != null && roots.Count != 0)
            {
                parent = roots.Pop();
                sb.Append(LocalName(parent)).Append('.');
            }
            return sb.Append(LocalName(node)).ToString();
        }

        protected internal override string LocalName(string name, bool escape = true)
        {
            return escape ? Escape(base.LocalName(name, escape)) : base.LocalName(name, escape);
        }
        private string LocalName(Schema.Node node)
        {
            string name = node.displayName;
            int prefixLen = (int)node.displayNamePrefixLength;
            if (prefixLen != 0) name = name.Substring(prefixLen);

            if (node.IsGroup())
            {
                return LocalName(name + "Group");
            }
            else
            {
                return LocalName(name);
            }
        }

        public override string Format(Schema.Type type, bool nullable = false)
        {
            if (type == null) return null;
            ulong typeid = 0;
            switch (type.Union)
            {
                case Schema.Type.Unions.anyPointer:
                    return Format(typeof(Pointer));
                case Schema.Type.Unions.@bool:
                    return nullable ? "bool?" : "bool";
                case Schema.Type.Unions.data:
                    return Format(typeof(Data));
                case Schema.Type.Unions.float32:
                    return nullable ? "float?" : "float";
                case Schema.Type.Unions.float64:
                    return nullable ? "double?" : "double";
                case Schema.Type.Unions.int16:
                    return nullable ? "short?" : "short";
                case Schema.Type.Unions.int32:
                    return nullable ? "int?" : "int";
                case Schema.Type.Unions.int64:
                    return nullable ? "long?" : "long";
                case Schema.Type.Unions.int8:
                    return nullable ? "sbyte?" : "sbyte";
                case Schema.Type.Unions.text:
                    return Format(typeof(Text));
                case Schema.Type.Unions.uint16:
                    return nullable ? "ushort?" : "ushort";
                case Schema.Type.Unions.uint32:
                    return nullable ? "uint?" : "uint";
                case Schema.Type.Unions.uint64:
                    return nullable ? "ulong?" : "ulong";
                case Schema.Type.Unions.uint8:
                    return nullable ? "byte?" : "byte";
                case Schema.Type.Unions.@void:
                    return Format(typeof(Void));
                case Schema.Type.Unions.@interface:
                    typeid = type.@interface.typeId.Value;
                    break;
                case Schema.Type.Unions.@struct:
                    typeid = type.@struct.typeId.Value;
                    break;
                case Schema.Type.Unions.@enum:
                    typeid = type.@enum.typeId.Value;
                    break;
            }
            Schema.Node node;
            if (typeid != 0 && (node = Lookup(typeid)) != null)
            {
                if (nullable)
                {
                    switch (node.Union)
                    {
                        case Schema.Node.Unions.@enum:
                            return FullyQualifiedName(node) + "?";
                        case Schema.Node.Unions.@struct:
                            if (node.IsGroup()) return FullyQualifiedName(node) + "?";
                            break;
                    }
                }
                return FullyQualifiedName(node);
            }

            if (type.Union == Schema.Type.Unions.list)
            {
                string el = Format(type.list.elementType);
                if (!string.IsNullOrWhiteSpace(el))
                {
                    return "global::" + typeof(FixedSizeList<>).Namespace + ".FixedSizeList<" + el + ">";
                }
            }
            return null;
        }

        public override CodeWriter WriteField(Schema.Field field)
        {
            var slot = field.slot;
            WriteLine().Write("[global::CapnProto.Field(").Write(slot.offset).Write(")]");

            WriteLine().Write("public ").Write(slot.type).Write(" ").Write(Escape(field.name)).Write(" {get; set; }");

            return this;
        }

        public override void WriteEnum(Schema.Node node)
        {
            if (node == null || node.Union != Schema.Node.Unions.@enum || node.@enum.enumerants == null) return;
            WriteLine().Write("public enum ").Write(LocalName(node));
            Indent();
            var items = node.@enum.enumerants;
            foreach (var item in items)
            {
                WriteLine().Write(Escape(item.name)).Write(" = ").Write(item.codeOrder).Write(",");
            }
            Outdent();
        }

        public override CodeWriter WriteLittleEndianCheck(Schema.Node node)
        {
            return this;
        }

        private void BeginProperty(Schema.Node type, string name, bool nullable)
        {
            WriteLine().Write("public ");
            Write(FullyQualifiedName(type)).Write(" ").Write(Escape(name));
            Indent();
        }
        private void BeginProperty(Schema.Type type, string name, bool nullable)
        {
            WriteLine().Write("public ");
            Write(Format(type)).Write(" ").Write(Escape(name));
            Indent();
        }
        public override CodeWriter WriteLiteral(byte value)
        {
            return Write("(byte)").Write(value);
        }
        public override CodeWriter WriteLiteral(ushort value)
        {
            return Write("(ushort)").Write(value);
        }
        public override CodeWriter WriteLiteral(uint value)
        {
            return Write("(uint)").Write(value);
        }
        public override CodeWriter WriteLiteral(sbyte value)
        {
            return Write("(sbyte)").Write(value);
        }
        public override CodeWriter WriteLiteral(short value)
        {
            return Write("(short)").Write(value);
        }
        public override CodeWriter WriteLiteral(int value)
        {
            return Write("(int)").Write(value);
        }
        public override CodeWriter WriteLiteral(long value)
        {
            return Write("(long)").Write(value);
        }       
        
        public override CodeWriter WriteLiteral(double value)
        {
            return Write(value).Write("D");
        }
        public override CodeWriter WriteLiteral(float value)
        {
            return Write(value).Write("F");
        }
        private CodeWriter WriteUnionTest(Stack<UnionStub> union)
        {
            if (union.Count == 0) return this;
            if (union.Count != 1) Write("(");
            bool first = true;
            foreach (var stub in union)
            {
                if (!first) Write(" && ");

                Write("this.").Write(PointerName).Write(".GetUInt16(").Write(stub.Offset).Write(") == ").WriteLiteral(stub.Value);
                first = false;
            }
            if (union.Count != 1) Write(")");
            return this;
        }
        public override CodeWriter WriteGroupAccessor(Schema.Node parent, Schema.Node child, string name, bool extraNullable)
        {
            string fieldOwner = parent.IsGroup() ? "this.parent" : "this";
            BeginProperty(child, name, extraNullable);
            WriteLine().Write("get");
            Indent();
            WriteLine().Write("return new ").Write(FullyQualifiedName(child)).Write("(").Write(fieldOwner).Write(");");
            Outdent();
            return Outdent();
        }

        const string PointerName = CodeWriter.PrivatePrefix;
        public override CodeWriter WriteFieldAccessor(Schema.Node parent, Schema.Field field, Stack<UnionStub> union)
        {
            if (field.Union == Field.Unions.group)
            {
                var found = Lookup(field.group.typeId);
                if (found == null)
                {
                    return WriteLine().Write("#warning no type for: " + field.name);
                }

                return WriteGroupAccessor(parent, found, field.name, union.Count != 0);
            }

            if (field.slot.type == null)
            {
                return WriteLine().Write("#warning no type for: " + field.name);
            }
            var ordinal = field.ordinal;
            var slot = field.slot;
            var type = slot.type;
            var len = type.GetFieldLength();
            if (len == 0) return this;

            if (ordinal.Union == Schema.Field.ordinalGroup.Unions.@explicit)
            {
                WriteLine().Write("[").Write(typeof(FieldAttribute)).Write("(").Write(ordinal.@explicit.Value);
                var offset = slot.offset;
                if (offset.HasValue)
                {
                    if (len == Schema.Type.LEN_POINTER)
                    {
                        Write(", pointer: ").Write(offset.Value);
                    }
                    else
                    {
                        int o = (int)offset.Value * len;
                        Write(", ").Write(o).Write(", ").Write(o + len);
                    }
                }
                Write(")]");
            }

            //bool extraNullable = union.Count != 0 && type.Union != Schema.Type.Unions.@struct;
            var grp = (len == Schema.Type.LEN_POINTER && type.Union == Schema.Type.Unions.@struct) ? Lookup(type.@struct.typeId) : null;
            if (grp != null && grp.IsGroup())
            {
                return WriteGroupAccessor(parent, grp, field.name, false);
            }
            if (slot.hadExplicitDefault.Value)
            {
                WriteLine().Write("[").Write(typeof(DefaultValueAttribute)).Write("(").Write(type, slot.defaultValue).Write(")]");
            }

            BeginProperty(type, field.name, false);
            WriteLine().Write("get");
            Indent();

            if (len == Schema.Type.LEN_POINTER)
            {
                // note: groups already handled
                WriteLine().Write("return (").Write(Format(slot.type)).Write(")this.").Write(PointerName).Write(".GetPointer(");
                WriteFieldOffset(slot.offset, union);
                Write(");");
            }
            else
            {   
                switch (type.Union)
                {
                    case Schema.Type.Unions.@bool:
                        WriteLine().Write("return ");
                        if (slot.hadExplicitDefault.Value && slot.defaultValue.Union == Schema.Value.Unions.@bool &&
                            slot.defaultValue.@bool.Value)
                        {
                            Write("!");
                        }
                        Write("this.").Write(PointerName).Write(".GetBoolean(");
                        WriteFieldOffset(slot.offset, union).Write(");");
                        break;
                    case Schema.Type.Unions.int8:
                    case Schema.Type.Unions.uint8:
                    case Schema.Type.Unions.int16:
                    case Schema.Type.Unions.uint16:
                    case Schema.Type.Unions.int32:
                    case Schema.Type.Unions.uint32:
                    case Schema.Type.Unions.int64:
                    case Schema.Type.Unions.uint64:
                    case Schema.Type.Unions.float32:
                    case Schema.Type.Unions.float64:
                        WriteLine().Write("return this.").Write(PointerName).Write(".");
                        switch(type.Union)
                        {
                            case Schema.Type.Unions.int8: Write("GetSByte"); break;
                            case Schema.Type.Unions.uint8: Write("GetByte"); break;
                            case Schema.Type.Unions.int16: Write("GetInt16"); break;
                            case Schema.Type.Unions.uint16: Write("GetUInt16"); break;
                            case Schema.Type.Unions.int32: Write("GetInt32"); break;
                            case Schema.Type.Unions.uint32: Write("GetUInt32"); break;
                            case Schema.Type.Unions.int64: Write("GetInt64"); break;
                            case Schema.Type.Unions.uint64: Write("GetUInt64"); break;
                            case Schema.Type.Unions.float32: Write("GetSingle"); break;
                            case Schema.Type.Unions.float64: Write("GetDouble"); break;
                        }
                        Write("(");
                        WriteFieldOffset(slot.offset, union).Write(")");
                        if (slot.hadExplicitDefault.Value)
                        {
                            WriteXorDefaultValue(field.slot.defaultValue);
                        }
                        Write(";");
                        break;
                    case Schema.Type.Unions.@enum:

                        var e = Lookup(type.@enum.typeId);
                        if (e == null || e.Union != Schema.Node.Unions.@enum || e.@enum.enumerants == null)
                        {
                            WriteLine().Write("#error enum not found: ").Write(type.@enum.typeId);
                        }
                        else
                        {
                            // all enums are Int16; so 4
                            WriteLine().Write("return (").Write(FullyQualifiedName(e)).Write(")this.")
                                .Write(PointerName).Write(".GetUInt16(");
                            WriteFieldOffset(slot.offset, union).Write(");");
                        }
                        break;
                    default:
                        WriteLine().Write("throw new global::System.NotImplementedException(); // ").Write(type.Union);
                        break;
                }
            }
            Outdent();

            
            WriteLine().Write("set");
            Indent();
            if (len == Schema.Type.LEN_POINTER)
            {
                WriteLine().Write("this.").Write(PointerName).Write(".SetPointer(");
                WriteFieldOffset(slot.offset, union).Write(", value);");
            }
            else
            {
                switch(type.Union)
                {
                    case Schema.Type.Unions.@bool:
                        WriteLine().Write("this.").Write(PointerName).Write(".SetBoolean(");
                        WriteFieldOffset(slot.offset, union).Write(", ");
                        if (slot.hadExplicitDefault.Value && slot.defaultValue.Union == Schema.Value.Unions.@bool &&
                            slot.defaultValue.@bool.Value)
                        {
                            Write("!");
                        }
                        Write("value);");
                        break;
                    case Schema.Type.Unions.int8:
                    case Schema.Type.Unions.uint8:
                    case Schema.Type.Unions.int16:
                    case Schema.Type.Unions.uint16:
                    case Schema.Type.Unions.int32:
                    case Schema.Type.Unions.uint32:
                    case Schema.Type.Unions.int64:
                    case Schema.Type.Unions.uint64:
                    case Schema.Type.Unions.float32:
                    case Schema.Type.Unions.float64:
                        WriteLine().Write("this.").Write(PointerName).Write(".");
                        switch(type.Union)
                        {
                            case Schema.Type.Unions.int8: Write("SetSByte"); break;
                            case Schema.Type.Unions.uint8: Write("SetByte"); break;
                            case Schema.Type.Unions.int16: Write("SetInt16"); break;
                            case Schema.Type.Unions.uint16: Write("SetUInt16"); break;
                            case Schema.Type.Unions.int32: Write("SetInt32"); break;
                            case Schema.Type.Unions.uint32: Write("SetUInt32"); break;
                            case Schema.Type.Unions.int64: Write("SetInt64"); break;
                            case Schema.Type.Unions.uint64: Write("SetUInt64"); break;
                            case Schema.Type.Unions.float32: Write("SetSingle"); break;
                            case Schema.Type.Unions.float64: Write("SetDouble"); break;
                        }
                        Write("(");
                        WriteFieldOffset(slot.offset, union).Write(", value");
                        if (slot.hadExplicitDefault.Value)
                        {
                            WriteXorDefaultValue(field.slot.defaultValue);
                        }
                        Write(");");
                        break;
                    case Schema.Type.Unions.@enum:

                        var e = Lookup(type.@enum.typeId);
                        if (e == null || e.Union != Schema.Node.Unions.@enum || e.@enum.enumerants == null)
                        {
                            WriteLine().Write("#error enum not found: ").Write(type.@enum.typeId);
                        }
                        else
                        {
                            WriteLine().Write("this.").Write(PointerName).Write(".SetUInt16(");
                            WriteFieldOffset(slot.offset, union).Write(", (ushort)value);");
                        }
                        break;
                    default:
                        WriteLine().Write("throw new global::System.NotImplementedException(); // ").Write(type.Union);
                        break;
                }
            }
            Outdent();
            return Outdent();
        }

        private void WriteXorDefaultValue(Value defaultValue)
        {
            if (defaultValue == null) return;
            switch (defaultValue.Union)
            {
                case Value.Unions.int8: if (defaultValue.int8.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.int8.Value); break;
                case Value.Unions.uint8: if (defaultValue.uint8.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.uint8.Value); break;
                case Value.Unions.int16: if (defaultValue.int16.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.int16.Value); break;
                case Value.Unions.uint16: if (defaultValue.uint16.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.uint16.Value); break;
                case Value.Unions.int32: if (defaultValue.int32.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.int32.Value); break;
                case Value.Unions.uint32: if (defaultValue.uint32.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.uint32.Value); break;
                case Value.Unions.int64: if (defaultValue.int64.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.int64.Value); break;
                case Value.Unions.uint64: if (defaultValue.uint64.Value != 0) Write(" ~ ").WriteLiteral(defaultValue.uint64.Value); break;
                default:
                    throw new NotSupportedException("Default value not supported for: " + defaultValue.Union);
            }
        }

        public override CodeWriter DeclareFields(int bodyWords, int pointers)
        {
            return this;
        }

        const string PointerPrefix = PrivatePrefix + "p_",
            DataPrefix = PrivatePrefix + "w_";
        public override CodeWriter Write(ulong value)
        {
            return Write("0x").Write(Convert.ToString(unchecked((long)value), 16));
        }

        public override CodeWriter WriteGroup(Schema.Node node, Stack<UnionStub> union)
        {
            var parent = node;
            while (parent != null && parent.IsGroup())
            {
                parent = FindParent(parent);
            }
            if (parent == null)
            {
                return WriteLine().Write("#error parent not found for: ").Write(node.displayName);
            }
            WriteLine().Write("[global::CapnProto.Group, global::CapnProto.Id(").Write(node.id).Write(")]");
            WriteLine().Write("public struct ").Write(LocalName(node));
            Indent();
            WriteLine().Write("private readonly ").Write(FullyQualifiedName(parent)).Write(" parent;");
            WriteLine().Write("internal ").Write(LocalName(node)).Write("(").Write(FullyQualifiedName(parent)).Write(" parent)");
            Indent();
            WriteLine().Write("this.parent = parent;");
            Outdent();
            if (node.@struct.fields != null)
            {
                foreach (var field in node.@struct.fields.OrderBy(x => x.codeOrder).ThenBy(x => x.name))
                {
                    if (field.discriminantValue == Field.noDiscriminant)
                    {
                        WriteFieldAccessor(node, field, union);
                    }
                    else
                    {
                        union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                        WriteFieldAccessor(node, field, union);
                        union.Pop();
                    }
                }
            }
            if (node.@struct.discriminantCount != 0)
            {
                WriteDiscriminant(node, union);
            }

            WriteNestedTypes(node, union);
            return Outdent();
        }

        public override CodeWriter WriteDiscriminant(Schema.Node node, Stack<UnionStub> union)
        {
            var @struct = node.@struct;
            WriteLine().Write("public ");
            if (union.Count != 0) Write("new ");
            Write("enum Unions");
            Indent();
            if (@struct.fields != null)
            {
                foreach (var field in @struct.fields)
                {
                    if (field.discriminantValue != Field.noDiscriminant)
                        WriteLine().Write(Escape(field.name)).Write(" = ").Write(field.discriminantValue).Write(",");
                }
            }
            Outdent();

            WriteLine().Write("public ").Write(FullyQualifiedName(node)).Write(".Unions Union");
            Indent();
            WriteLine().Write("get");
            Indent();
            WriteLine().Write("return (").Write(FullyQualifiedName(node)).Write(".Unions)this.").Write(PointerName).Write(".GetUInt16(");
            WriteFieldOffset(node.@struct.discriminantOffset, union).Write(");");
            Outdent();
            //WriteLine().Write("set");
            //Indent();
            //ulong mask = ~((ulong)0xFFFF << byteInWord);
            //WriteLine().Write(node.IsGroup() ? "this.parent" : "this").Write(".").Write(DataPrefix).Write(wordIndex).Write(" = (")
            //    .Write(node.IsGroup() ? "this.parent" : "this").Write(".").Write(DataPrefix).Write(wordIndex).Write(" & ").Write(mask).Write(") | ");
            //if (byteInWord == 0) Write("(ulong)value;");
            //else Write("((ulong)value << ").Write(byteInWord).Write(");");
            //Outdent();


            //foreach(var field in @struct.fields)
            //{
            //    if(field.discriminantValue != Field.noDiscriminant && field.slot.type.Union == Schema.Type.Unions.@struct)
            //    {
            //        var found = Lookup(field.slot.type.@struct.typeId);
            //        if(found != null && found.IsGroup())
            //        {
            //            union.Push(new UnionStub(@struct.discriminantOffset, field.discriminantValue));

            //            union.Pop();
            //        }
            //    }
            //}

            return Outdent();
        }

        CodeWriter WriteFieldOffset(uint? index, Stack<UnionStub> union)
        {
            return union.Count == 0 ? Write(index)
                : WriteUnionTest(union).Write(" ? ").Write(index).Write(" : -1");
        }

        public override CodeWriter WriteEnumLiteral(Schema.Type type, ushort value)
        {
            return Write("(").Write(type).Write(")").Write(value);
        }
    }
}
