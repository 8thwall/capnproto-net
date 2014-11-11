using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CapnProto.Schema;
using System;
using System.ComponentModel;
using System.Linq;
namespace CapnProto
{
    public class CSharpCodeWriter : CodeWriter
    {
        public override bool NeedsSerializer
        {
            get { return true; }
        }
        public CSharpCodeWriter(TextWriter destination, List<Schema.Node> nodes,
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
            return Write("@\"").Write(value.Replace("\"","\"\"")).Write("\"");
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
                //if (node.@struct.isGroup)
                //{
                //    WriteLine().Write("[global::CapnProto.Group]");
                //}
                if (node.id != 0)
                {
                    WriteLine().Write("[global::CapnProto.Id(").Write(node.id).Write(")]");
                }
            }
            WriteLine().Write("public partial class ").Write(LocalName(node)).Write(" : ").Write(typeof(IBlittable));
            Indent()
                .WriteLine().Write("static partial void OnCreate(ref ").Write(FullyQualifiedName(node)).Write(" obj);")
                .WriteLine().Write("internal static ").Write(FullyQualifiedName(node)).Write(" " + Ctor + "()");
            Indent()
                .WriteLine().Write(FullyQualifiedName(node)).Write(" tmp = null;")
                .WriteLine().Write("OnCreate(ref tmp);")
                .WriteLine().Write("// if you are providing custom construction, please also provide a private")
                .WriteLine().Write("// parameterless constructor (it can just throw an exception if you like)")
                .WriteLine().Write("return tmp ?? new ").Write(FullyQualifiedName(node)).Write("();");

            Outdent();
            WriteBlit(node);
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
        private void WriteBlit(Schema.Node node)
        {
            WriteLine().Write("unsafe void ").Write(typeof(IBlittable)).Write(".Deserialize(int segment, int origin, ").Write(typeof(DeserializationContext)).Write(" ctx, ulong pointer)");
            Indent();
            int bodyWords = 0, pointerWords = 0;
            Schema.CodeGeneratorRequest.ComputeSpace(this, node, ref bodyWords, ref pointerWords);
            var ptrFields = new SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>>();
            //List<Schema.Field> lists = new List<Schema.Field>();
            var union = new Stack<UnionStub>();
            CascadePointers(this, node, ptrFields, union);
            int alloc = Math.Max(bodyWords, pointerWords);
            if (alloc != 0)
            {
                WriteLine().Write("ulong* raw = stackalloc ulong[").Write(alloc).Write("];");
                if (bodyWords != 0)
                {
                    WriteLine().Write("ctx.Reader.ReadData(segment, origin, pointer, raw, ").Write(bodyWords).Write(");");
                    for (int i = 0; i < bodyWords; i++)
                    {
                        WriteLine().Write(DataPrefix).Write(i).Write(" = raw[").Write(i).Write("];");
                    }
                }
                if (pointerWords != 0)
                {
                    WriteBlitPointers(node, pointerWords, ptrFields); //, lists);
                }
            }
            Outdent();
            //int listIndex = 0;
            //foreach (var listField in lists)
            //{
            //    WriteLine().Write("static object ").Write(ListMethodName(listIndex++)).Write("(int segment, int origin, ")
            //        .Write(typeof(CapnProto.DeserializationContext)).Write(" ctx, ulong pointer)");
            //    Indent();
            //    WriteListImpl(listField, false);
            //    Outdent();
            //}
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

        private void WriteBlitPointers(Schema.Node node, int pointerWords, SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>> ptrFields) //, List<Schema.Field> lists)
        {
            WriteLine().Write("origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, ").Write(pointerWords).Write(");");
            foreach (var pair in ptrFields)
            {
                WriteLine().Write("if (raw[").Write(pair.Key).Write("] != 0)");
                Indent();
                foreach (var tuple in pair.Value)
                {
                    var declaring = tuple.Item1;
                    var field = tuple.Item2;
                    var union = tuple.Item3;
                    if (field.slot.type == null) continue;
                    Schema.Node found = null;
                    if (field.slot.type.Union == Schema.Type.Unions.@struct)
                    {
                        found = Lookup(field.slot.type.@struct.typeId);
                        if (found == null || found.Union != Schema.Node.Unions.@struct)
                        {
                            WriteLine().Write("#warning not found: ").Write(field.slot.type.@struct.typeId);
                            continue;
                        }

                        if (found.IsGroup()) continue; // group data is included separately at the correct locations
                    }
                    WriteLine().Write("// ").Write(declaring.displayName).Write(".").Write(field.name).WriteLine();
                    if (union.Count != 0)
                    {
                        Write("if (");
                        WriteUnionTest("this", union);
                        Write(") ");
                    }
                    Write("this." + PointerPrefix).Write(pair.Key).Write(" = ");
                    switch (field.slot.type.Union)
                    {
                        case Schema.Type.Unions.text:

                            Write("ctx.Reader.ReadStringFromPointer(segment, origin + ").Write(field.slot.offset + 1);
                            Write(", raw[").Write(field.slot.offset).Write("]);");
                            break;
                        case Schema.Type.Unions.data:

                            Write("ctx.Reader.ReadBytesFromPointer(segment, origin + ").Write(field.slot.offset + 1);
                            Write(", raw[").Write(field.slot.offset).Write("]);");
                            break;
                        case Schema.Type.Unions.@struct:
                            if (found != null)
                            {
                                Write("global::").Write(Namespace).Write(".").Write(Escape(Serializer)).Write(".")
                                    .Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(".")
                                    .Write(found.CustomSerializerName()).Write("(segment, origin + ").Write(field.slot.offset + 1);
                                Write(", ctx, raw[").Write(field.slot.offset).Write("]);");
                            }
                            break;
                        case Schema.Type.Unions.list:
                            WriteListImpl(field);
                            //if (!WriteListImpl(field, true))
                            //{
                            //    //Write("global::").Write(Namespace).Write(".").Write(Escape(Serializer)).Write(".")
                            //    //    .Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(".")
                            //    Write(ListMethodName(lists.Count))
                            //    .Write("(segment, origin");
                            //    lists.Add(field);
                            //    if (field.slot.offset != 0) Write(" + ").Write(field.slot.offset);
                            //    Write(", ctx, raw[").Write(field.slot.offset).Write("]);");
                            //}
                            break;
                        case Schema.Type.Unions.anyPointer:
                            Write("null;").WriteLine().Write("#warning any-pointer not yet implemented");
                            break;
                        default:
                            Write("null;").WriteLine().Write("#warning unexpected type: " + field.slot.type.Union);
                            break;
                    }
                }
                Outdent();
            }
        }
        private static string ListMethodName(int index)
        {
            return CodeWriter.PrivatePrefix + "l_" + index.ToString(CultureInfo.InvariantCulture);
        }
        const string Ctor = PrivatePrefix + "ctor";


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

        public override CodeWriter WriteSerializerTest(string field, Schema.Node node, string serializer)
        {
            return WriteLine().Write("if (type == typeof(").Write(FullyQualifiedName(node)).Write(")) return ").Write(field)
                .Write(" ?? (").Write(field).Write(" = new ").Write(serializer).Write("(this));");
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
        public override CodeWriter WriteCustomReaderMethod(Schema.Node node)
        {
            var fqn = FullyQualifiedName(node);
            WriteLine().Write("internal static ").Write(fqn).Write(" ").Write(node.CustomSerializerName()).Write("(int segment, int origin, ").Write(typeof(DeserializationContext)).Write(" ctx, ulong pointer)");
            Indent();
            WriteLine().Write("var obj = ").Write(fqn).Write("." + Ctor + "();");
            WriteLine(false).Write("#pragma warning disable 0618");
            WriteLine().Write(typeof(TypeSerializer)).Write(".Deserialize<").Write(fqn).Write(">(ref obj, segment, origin, ctx, pointer);");
            WriteLine(false).Write("#pragma warning restore 0618");
            WriteLine().Write("return obj;");

            //foreach (var field in node.@struct.fields)
            //{
            //    var slot = field.slot;
            //    if (slot == null || slot.type == null) continue;
            //    int len = slot.type.GetFieldLength();
            //    WriteLine().Write("// ");
            //    var ord = field.ordinal;
            //    if (ord != null && ord.@implicit == null)
            //    {
            //        Write("@").Write(ord.@explicit).Write(" ");
            //    }
            //    Write(field.name).Write(": ");
            //    switch (len)
            //    {
            //        case 0:
            //            Write("nothing to do");
            //            break;
            //        case -1:
            //            Write("pointer ").Write(slot.offset);
            //            break;
            //        default:
            //            int start = checked(len * (int)slot.offset);
            //            Write("bit ").Write(start).Write(" (word ").Write(start / 64).Write(" shift ").Write(start % 64).Write(")");
            //            break;
            //    }
            //}
            //WriteLine().Write("throw new global::System.NotImplementedException();");

            return EndMethod();
        }

        public override string Format(Schema.Type type, bool nullable = false)
        {
            if (type == null) return null;
            ulong typeid = 0;
            switch (type.Union)
            {
                case Schema.Type.Unions.anyPointer:
                    return "object";
                case Schema.Type.Unions.@bool:
                    return nullable ? "bool?" : "bool";
                case Schema.Type.Unions.data:
                    return "byte[]";
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
                    return "string";
                case Schema.Type.Unions.uint16:
                    return nullable ? "ushort?" : "ushort";
                case Schema.Type.Unions.uint32:
                    return nullable ? "uint?" : "uint";
                case Schema.Type.Unions.uint64:
                    return nullable ? "ulong?" : "ulong";
                case Schema.Type.Unions.uint8:
                    return nullable ? "byte?" : "byte";
                case Schema.Type.Unions.@void:
                    return "global::CapnProto.Void";
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
                    return "global::System.Collections.Generic.List<" + el + ">";
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
            WriteLine().Write("static ").Write(LocalName(node)).Write("()");
            return Indent().WriteLine().Write(typeof(TypeModel)).Write(".AssertLittleEndian();").EndMethod();
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
            if ((type.float32 ?? type.float64) != null) Write("unsafe ");
            Write(Format(type, nullable)).Write(" ").Write(Escape(name));
            Indent();
        }
        private CodeWriter WriteUnionTest(string fieldOwner, Stack<UnionStub> union)
        {
            if (union.Count == 0) return this;
            if (union.Count != 1) Write("(");
            bool first = true;
            foreach (var stub in union)
            {
                if (!first) Write(" && ");
                
                Write("(").Write(fieldOwner).Write("." + DataPrefix).Write(stub.FieldIndex).Write(" & ")
                    .Write(stub.Mask).Write(") == ").Write(stub.Expected);
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
        public override CodeWriter WriteFieldAccessor(Schema.Node parent, Schema.Field field, Stack<UnionStub> union)
        {
            string fieldOwner = parent.IsGroup() ? "this.parent" : "this";

            if(field.Union == Field.Unions.group)
            {
                var found = Lookup(field.group.typeId);
                if(found == null)
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
            if (ordinal.Union == Schema.Field.ordinalGroup.Unions.@explicit)
            {
                WriteLine().Write("[").Write(typeof(FieldAttribute)).Write("(").Write(ordinal.@explicit.Value);
                var offset = slot.offset;
                if(offset.HasValue)
                {
                    if(len == Schema.Type.LEN_POINTER)
                    {
                        Write(", pointer: ").Write(offset.Value);
                    } else
                    {
                        int o = (int)offset.Value * len;
                        Write(", ").Write(o).Write(", ").Write(o + len);
                    }
                }
                Write(")]");
            }
            
            bool extraNullable = union.Count != 0 && type.Union != Schema.Type.Unions.@struct;
            var grp = (len == Schema.Type.LEN_POINTER && type.Union == Schema.Type.Unions.@struct) ? Lookup(type.@struct.typeId) : null;
            if (grp != null && grp.IsGroup())
            {
                return WriteGroupAccessor(parent, grp, field.name, extraNullable);
            }
            if(slot.hadExplicitDefault.Value)
            {
                WriteLine().Write("[").Write(typeof(DefaultValueAttribute)).Write("(").Write(type, slot.defaultValue).Write(")]");
            }

            BeginProperty(type, field.name, extraNullable);
            WriteLine().Write("get");
            Indent();

            if (len == 0)
            {
                WriteLine().Write("return ");
                if (union.Count != 0) WriteUnionTest(fieldOwner, union).Write(" ? ");
                Write(typeof(Void)).Write(".Value");
                if (union.Count != 0) Write(" : null");
                Write(";");
            }
            else if (len == Schema.Type.LEN_POINTER)
            {
                // note: groups already handled
                WriteLine().Write("return ");
                if (union.Count != 0) WriteUnionTest(fieldOwner, union).Write(" ? ");
                Write("(").Write(Format(type)).Write(")").Write(fieldOwner).Write("." + PointerPrefix).Write(slot.offset);
                if (union.Count != 0) Write(" : null");
                Write(";");
            }
            else
            {
                int byteInData = checked((int)slot.offset * len), byteInWord = byteInData % 64;

                string fieldName = DataPrefix + (byteInData / 64);
                ulong mask;
                if (union.Count != 0)
                {
                    WriteLine().Write("if (");
                    WriteUnionTest(fieldOwner, union).Write(")");
                    Indent();
                }
                switch (type.Union)
                {

                    case Schema.Type.Unions.@bool:

                        mask = ((ulong)1) << byteInWord;
                        WriteLine().Write("return (").Write(fieldOwner).Write(".").Write(fieldName).Write(" & ").Write(mask);
                        if (slot.hadExplicitDefault.Value && slot.defaultValue.Union == Schema.Value.Unions.@bool &&
                            slot.defaultValue.@bool.Value)
                        {
                            Write(") == 0;");
                        }
                        else
                        {
                            Write(") != 0;");
                        }
                        break;
                    case Schema.Type.Unions.int8:
                    case Schema.Type.Unions.uint8:
                    case Schema.Type.Unions.int16:
                    case Schema.Type.Unions.uint16:
                    case Schema.Type.Unions.int32:
                    case Schema.Type.Unions.uint32:
                    case Schema.Type.Unions.int64:
                    case Schema.Type.Unions.uint64:
                        WriteLine().Write("return unchecked((").Write(type).Write(")(");
                        if (byteInWord == 0) Write(fieldOwner).Write(".").Write(fieldName);
                        else Write("(").Write(fieldOwner).Write(".").Write(fieldName).Write(" >> ").Write(byteInWord).Write(")");
                        if (slot.hadExplicitDefault.Value)
                        {
                            WriteXorDefaultValue(field.slot.defaultValue, 0); // passing byteInWord = 0 because already shifted
                        }
                        Write("));");
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
                            mask = ((ulong)0xFFFF) << byteInWord;

                            WriteLine().Write("switch(").Write(fieldOwner).Write(".").Write(fieldName).Write(" & ").Write(mask).Write(")");
                            Indent();
                            foreach (var enumerant in e.@enum.enumerants)
                            {
                                WriteLine().Write("case ").Write((ulong)enumerant.codeOrder << byteInWord).Write(": return ").Write(FullyQualifiedName(e)).Write(".").Write(Escape(enumerant.name)).Write(";");
                            }
                            WriteLine().Write("default: throw new global::System.InvalidOperationException(\"unexpected enum value: \" + unchecked((ushort)(")
                                .Write(fieldOwner).Write(".").Write(fieldName).Write(" >> ").Write(byteInWord).Write(")));");
                            Outdent();
                        }
                        break;
                    case Schema.Type.Unions.float32:
                    case Schema.Type.Unions.float64:
                        WriteLine().Write(typeof(ulong)).Write(" tmp = ").Write(fieldOwner).Write(".").Write(fieldName);
                        if (byteInWord != 0) Write(" >> ").Write(byteInWord);
                        Write(";").WriteLine().Write("return *((").Write(type).Write("*)(&tmp));");
                        break;
                    default:
                        WriteLine().Write("throw new global::System.NotImplementedException(); // ").Write(type.Union);
                        break;
                }
                if (union.Count != 0)
                {
                    Outdent();
                    WriteLine().Write("return null;");
                }
            }
            Outdent();

            if (len == 0)
            {
                // nothing to do
            }
            else if (len == Schema.Type.LEN_POINTER)
            {
                // note: groups don't get setters
                WriteLine().Write("set");
                Indent();
                if (union.Count != 0)
                {
                    WriteLine().Write("if(!(");
                    WriteUnionTest(fieldOwner, union).Write(")) throw new ").Write(typeof(InvalidUnionDiscriminatorException)).Write("();");
                }
                WriteLine().Write(fieldOwner).Write("." + PointerPrefix).Write(slot.offset).Write(" = value;");
                Outdent();
            }
            else
            {
                int byteInData = checked((int)slot.offset * len), byteInWord = byteInData % 64;

                string fieldNameIncludingOwner = fieldOwner + "." + DataPrefix + (byteInData / 64);
                WriteLine().Write("set");
                Indent();

                if (union.Count != 0)
                {
                    WriteLine().Write("if(!(");
                    WriteUnionTest(fieldOwner, union).Write(")) throw new ").Write(typeof(InvalidUnionDiscriminatorException)).Write("();");
                }
                ulong mask;
                switch (type.Union)
                {
                    case Schema.Type.Unions.@bool:
                        mask = ((ulong)1) << byteInWord;
                        WriteLine().Write("if(value");
                        if (extraNullable) Write(".Value");
                        Write(")");
                        Indent();
                        WriteLine().Write(fieldNameIncludingOwner).Write(" |= ").Write(mask).Write(";");
                        Outdent();
                        WriteLine().Write("else");
                        Indent();
                        WriteLine().Write(fieldNameIncludingOwner).Write(" &= ").Write(~mask).Write(";");
                        Outdent();
                        break;
                    case Schema.Type.Unions.uint64:
                        WriteLine().Write(fieldNameIncludingOwner).Write(" = value").Write(extraNullable ? ".Value;" : ";");
                        break;
                    case Schema.Type.Unions.int64:
                        WriteLine().Write(fieldNameIncludingOwner).Write(" = unchecked((ulong)value").Write(extraNullable ? ".Value);" : ");");
                        break;
                    case Schema.Type.Unions.int8:
                    case Schema.Type.Unions.uint8:
                    case Schema.Type.Unions.int16:
                    case Schema.Type.Unions.uint16:
                    case Schema.Type.Unions.int32:
                    case Schema.Type.Unions.uint32:
                        mask = 0;
                        if ((type.int8 ?? type.uint8) != null) mask = 0xFF;
                        else if ((type.int16 ?? type.uint16) != null) mask = 0xFFFF;
                        else if ((type.int32 ?? type.uint32) != null) mask = 0xFFFFFFFF;
                        mask = ~(mask << byteInWord);

                        WriteLine().Write(fieldNameIncludingOwner).Write(" = (").Write(fieldNameIncludingOwner).Write(" & ").Write(mask)
                            .Write(") | unchecked(((ulong)(value");
                        if (extraNullable) Write(".Value");
                        Write(")");
                        if (byteInWord != 0) Write(" << ").Write(byteInWord);
                        Write("));");
                        break;
                    default:
                        WriteLine().Write("throw new global::System.NotImplementedException(); // ").Write(type.Union);
                        break;
                }

                //                else if (type.@enum != null && len == 16)
                //                {
                //                    var e = Lookup(type.@enum.typeId);
                //                    if (e == null || e.@enum == null || e.@enum.enumerants == null)
                //                    {
                //                        WriteLine().Write("#error enum not found: ").Write(type.@enum.typeId);
                //                    }
                //                    else
                //                    {
                //                        // all enums are Int16; so 4 
                //                        ulong mask = ((ulong)0xFFFF) << byteInWord;
                //
                //                        WriteLine().Write("switch(").Write(fieldOwner).Write(".").Write(fieldName).Write(" & ").Write(mask).Write(")");
                //                        Indent();
                //                        foreach (var enumerant in e.@enum.enumerants)
                //                        {
                //                            WriteLine().Write("case ").Write((ulong)enumerant.codeOrder << byteInWord).Write(": return ").Write(FullyQualifiedName(e)).Write(".").Write(Escape(enumerant.name)).Write(";");
                //                        }
                //                        WriteLine().Write("default: throw new global::System.InvalidOperationException(\"unexpected enum value: \" + unchecked((ushort)(")
                //                            .Write(fieldOwner).Write(".").Write(fieldName).Write(" >> ").Write(byteInWord).Write(")));");
                //                        Outdent();
                //                    }
                //                }
                //                else if ((type.float32 ?? type.float64) != null)
                //                {
                //                    WriteLine().Write(typeof(ulong)).Write(" tmp = ").Write(fieldOwner).Write(".").Write(fieldName);
                //                    if (byteInWord != 0) Write(" >> ").Write(byteInWord);
                //                    Write(";").WriteLine().Write("return *((").Write(type).Write("*)(&tmp));");
                //                }
                Outdent();
            }
            return Outdent();
        }

        private void WriteXorDefaultValue(Value defaultValue, int byteInWord)
        {
            if (defaultValue == null) return;
            ulong value; ;
            switch(defaultValue.Union)
            {
                case Value.Unions.@void: value = 0; break;
                case Value.Unions.@bool: value = defaultValue.@bool.Value ? (ulong)1 : 0; break;
                case Value.Unions.int8: value = unchecked((ulong)defaultValue.int8.Value); break;
                case Value.Unions.uint8: value = unchecked((ulong)defaultValue.uint8.Value); break;
                case Value.Unions.int16: value = unchecked((ulong)defaultValue.int16.Value); break;
                case Value.Unions.uint16: value = unchecked((ulong)defaultValue.uint16.Value); break;
                case Value.Unions.int32: value = unchecked((ulong)defaultValue.int32.Value); break;
                case Value.Unions.uint32: value = unchecked((ulong)defaultValue.uint32.Value); break;
                case Value.Unions.int64: value = unchecked((ulong)defaultValue.int64.Value); break;
                case Value.Unions.uint64: value = unchecked((ulong)defaultValue.uint64.Value); break;
                default:
                    throw new NotSupportedException("Default value not supported for: " + defaultValue.Union);
            }
            if (value != 0)
                Write(" ^ ").Write(value << byteInWord);
        }

        public override CodeWriter DeclareFields(int bodyWords, int pointers)
        {
            return WriteLine().Write("// ").Write("body words: ").Write(bodyWords)
                      .Write("; pointers: ").Write(pointers)
                      .DeclareFields(DataPrefix, bodyWords, typeof(ulong))
                      .DeclareFields(PointerPrefix, pointers, typeof(object));
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
            WriteLine().Write("[").Write(typeof(GroupAttribute)).Write(", ").Write(typeof(IdAttribute)).Write("(").Write(node.id).Write(")]");
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
            var offset = (int)(@struct.discriminantOffset * 16);
            int wordIndex = offset / 64, byteInWord = offset % 64;
            WriteLine().Write("return (").Write(FullyQualifiedName(node)).Write(".Unions)((").Write(node.IsGroup() ? "this.parent" : "this").Write(".")
                .Write(DataPrefix).Write(wordIndex);
            if (byteInWord != 0) Write(" >> ").Write(byteInWord);
            Write(") & 0xFFFF);");
            Outdent();
            WriteLine().Write("set");
            Indent();
            ulong mask = ~((ulong)0xFFFF << byteInWord);
            WriteLine().Write(node.IsGroup() ? "this.parent" : "this").Write(".").Write(DataPrefix).Write(wordIndex).Write(" = (")
                .Write(node.IsGroup() ? "this.parent" : "this").Write(".").Write(DataPrefix).Write(wordIndex).Write(" & ").Write(mask).Write(") | ");
            if (byteInWord == 0) Write("(ulong)value;");
            else Write("((ulong)value << ").Write(byteInWord).Write(");");
            Outdent();


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

        public override CodeWriter WriteEnumLiteral(Schema.Type type, ushort value)
        {
            return Write("(").Write(type).Write(")").Write(value);
        }
    }
}
