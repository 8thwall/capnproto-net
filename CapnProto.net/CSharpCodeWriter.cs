using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CapnProto
{
    public class CSharpCodeWriter : CodeWriter
    {
        public CSharpCodeWriter(TextWriter destination, List<Schema.Node> nodes,
            string @namespace, string serializer)
            : base(destination, nodes, @namespace, serializer) { }
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

        public override CodeWriter BeginClass(Schema.Node node)
        {
            if (node.@struct != null)
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
            WriteLine().Write("unsafe void ").Write(typeof(IBlittable)).Write(".Deserialize(int segment, int origin, global::CapnProto.CapnProtoReader reader, ulong pointer)");
            Indent();
            int bodyWords = 0, pointerWords = 0;
            Schema.CodeGeneratorRequest.ComputeSpace(this, node, ref bodyWords, ref pointerWords);
            var ptrFields = new SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>>();
            List<Schema.Field> lists = new List<Schema.Field>();
            var union = new Stack<UnionStub>();
            CascadePointers(this, node, ptrFields, union);
            int alloc = Math.Max(bodyWords, pointerWords);
            if (alloc != 0)
            {
                WriteLine().Write("ulong* raw = stackalloc ulong[").Write(alloc).Write("];");
                if (bodyWords != 0)
                {
                    WriteLine().Write("reader.ReadData(segment, origin, pointer, raw, ").Write(bodyWords).Write(");");
                    for (int i = 0; i < bodyWords; i++)
                    {
                        WriteLine().Write(DataPrefix).Write(i).Write(" = raw[").Write(i).Write("];");
                    }
                }
                if (pointerWords != 0)
                {
                    WriteBlitPointers(node, pointerWords, ptrFields, lists);
                }
            }
            Outdent();
            int listIndex = 0;
            foreach (var listField in lists)
            {
                WriteLine().Write("static object ").Write(ListMethodName(listIndex++)).Write("(int segment, int origin, ")
                .Write(typeof(CapnProto.CapnProtoReader)).Write(" reader, ulong pointer)");
                Indent();
                WriteLine().Write("return null;");
                Outdent();
            }
        }

        private static void CascadePointers(CodeWriter writer, Schema.Node node, SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>> ptrFields, Stack<UnionStub> union)
        {
            foreach (var field in node.@struct.fields)
            {
                var slot = field.slot;
                if (slot == null || slot.type == null) continue;
                int len = slot.type.GetFieldLength();

                if (len == Schema.Type.LEN_POINTER)
                {
                    bool isFiltered = field.discriminantValue != ushort.MaxValue;
                    if (isFiltered) union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                    List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>> fields;
                    if (!ptrFields.TryGetValue((int)slot.offset, out fields))
                    {
                        ptrFields.Add((int)slot.offset, fields = new List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>());
                    }
                    fields.Add(Tuple.Create(node, field, Clone(union)));

                    if (slot.type.@struct != null)
                    {
                        var found = writer.Lookup(slot.type.@struct.typeId);
                        if (found != null && found.@struct != null && found.@struct.isGroup)
                        {
                            CascadePointers(writer, found, ptrFields, union);
                        }
                    }
                    if (isFiltered) union.Pop();
                }
            }
        }

        private void WriteBlitPointers(Schema.Node node, int pointerWords, SortedList<int, List<Tuple<Schema.Node, Schema.Field, Stack<UnionStub>>>> ptrFields, List<Schema.Field> lists)
        {
            WriteLine().Write("origin = reader.ReadPointers(segment, origin, pointer, raw, ").Write(pointerWords).Write(");");
            foreach (var pair in ptrFields)
            {
                WriteLine().Write("if (raw[").Write(pair.Key).Write("] != 0)");
                Indent();
                foreach (var tuple in pair.Value)
                {
                    var declaring = tuple.Item1;
                    var field = tuple.Item2;
                    var union = tuple.Item3;
                    if(field.slot == null || field.slot.type == null) continue;
                    Schema.Node found = null;
                    if(field.slot.type.@struct != null)
                    {
                        found = Lookup(field.slot.type.@struct.typeId);
                        if (found == null || found.@struct == null)
                        {
                            WriteLine().Write("#warning not found: ").Write(field.slot.type.@struct.typeId);
                            continue;
                        }

                        if (found.@struct.isGroup) continue; // group data is included separately at the correct locations
                    }
                    WriteLine().Write("// ").Write(declaring.displayName).Write(".").Write(field.name).WriteLine();
                    if (union.Count != 0)
                    {
                        Write("if (");
                        WriteUnionTest("this", union);
                        Write(") ");
                    }
                    Write("this." + PointerPrefix).Write(pair.Key).Write(" = ");
                    if (field.slot.type.text != null)
                    {
                        Write("reader.ReadStringFromPointer(segment, origin");
                        if (field.slot.offset != 0) Write(" + ").Write(field.slot.offset);
                        Write(", raw[").Write(field.slot.offset).Write("]);");
                    }
                    else if (field.slot.type.data != null)
                    {
                        Write("reader.ReadBytesFromPointer(segment, origin");
                        if (field.slot.offset != 0) Write(" + ").Write(field.slot.offset);
                        Write(", raw[").Write(field.slot.offset).Write("]);");
                    }
                    else if (field.slot.type.@struct != null && found != null)
                    {
                        Write("global::").Write(Namespace).Write(".").Write(Escape(Serializer)).Write(".")
                            .Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(".")
                            .Write(found.CustomSerializerName()).Write("(segment, origin");
                        if (field.slot.offset != 0) Write(" + ").Write(field.slot.offset);
                        Write(", reader, raw[").Write(field.slot.offset).Write("]);");
                    }
                    else if (field.slot.type.list != null)
                    {
                        //Write("global::").Write(Namespace).Write(".").Write(Escape(Serializer)).Write(".")
                        //    .Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(".")
                            Write(ListMethodName(lists.Count))
                            .Write("(segment, origin");
                        lists.Add(field);
                        if (field.slot.offset != 0) Write(" + ").Write(field.slot.offset);
                        Write(", reader, raw[").Write(field.slot.offset).Write("]);");
                    }
                    else if (field.slot.type.anyPointer != null)
                    {
                        Write("null;").WriteLine().Write("#warning any-pointer not yet implemented");
                    }
                    else
                    {
                        Write("null;").WriteLine().Write("#warning unexpected type");
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

        public override CodeWriter BeginClass(bool @public, bool @internal, string name, Type baseType)
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

        public override CodeWriter DeclareField(string name, Type type)
        {
            return WriteLine().Write("private ").Write(type).Write(" ").Write(Escape(name)).Write(";");
        }
        public override CodeWriter DeclareFields(string prefix, int count, Type type)
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
        public override CodeWriter DeclareFields(List<string> names, Type type)
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
                .Write(" ?? (").Write(field).Write(" = new ").Write(serializer).Write("());");
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

        public override CodeWriter Write(Type type)
        {
            return Write(Format(type));
        }
        private string Format(Type type)
        {
            if (type == null || type == typeof(void)) return "void";
            if (!type.IsEnum) // reports same typecodes
            {
                switch (Type.GetTypeCode(type))
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
            if (type == typeof(object)) return "object"; // cant use TypeCode.Object: most classes etc report that
            return "global::" + type.FullName.Replace('+', '.');
        }

        public override CodeWriter WriteCustomSerializerClass(Schema.Node node, string typeName, string methodName)
        {
            WriteLine().Write("class ").Write(typeName).Write(" : global::").Write(Namespace).Write(".").Write(Escape(Serializer))
                .Write(".").Write(Schema.CodeGeneratorRequest.BaseTypeName).Write(", global::CapnProto.ITypeSerializer<").Write(FullyQualifiedName(node)).Write(">");
            Indent();

            WriteLine().Write("object global::CapnProto.ITypeSerializer.Deserialize(global::CapnProto.CapnProtoReader reader)");
            Indent().WriteLine().Write("return Deserialize(reader);");
            EndMethod();

            WriteLine().Write("public ").Write(FullyQualifiedName(node)).Write(" Deserialize(global::CapnProto.CapnProtoReader reader)");
            Indent().WriteLine().Write("reader.ReadPreamble();").WriteLine().Write("return ").Write(methodName).Write("(0, 1, reader, reader.ReadWord(0, 0));");
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
            if (parent != null)
            {
                roots = new Stack<Schema.Node>();
                do
                {
                    roots.Push(parent);
                    parent = FindParent(parent);
                } while (parent != null);
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

        private string LocalName(Schema.Node node)
        {
            return Escape(node.displayName);
        }
        public override CodeWriter WriteCustomReaderMethod(Schema.Node node)
        {
            var fqn = FullyQualifiedName(node);
            WriteLine().Write("internal static ").Write(fqn).Write(" ").Write(node.CustomSerializerName()).Write("(int segment, int origin, global::CapnProto.CapnProtoReader reader, ulong pointer)");
            Indent();
            WriteLine().Write("var obj = ").Write(fqn).Write("." + Ctor + "();");
            WriteLine(false).Write("#pragma warning disable 0618");
            WriteLine().Write(typeof(TypeSerializer)).Write(".Deserialize<").Write(fqn).Write(">(ref obj, segment, origin, reader, pointer);");
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
            if (type.anyPointer != null) return "object";
            if (type.@bool != null) return nullable ? "bool?" : "bool";
            if (type.data != null) return "byte[]";
            if (type.float32 != null) return nullable ? "float?" : "float";
            if (type.float64 != null) return nullable ? "double?" : "double";
            if (type.int16 != null) return nullable ? "short?" : "short";
            if (type.int32 != null) return nullable ? "int?" : "int";
            if (type.int64 != null) return nullable ? "long?" : "long";
            if (type.int8 != null) return nullable ? "sbyte?" : "sbyte";
            if (type.text != null) return "string";
            if (type.uint16 != null) return nullable ? "ushort?" : "ushort";
            if (type.uint32 != null) return nullable ? "uint?" : "uint";
            if (type.uint64 != null) return nullable ? "ulong?" : "ulong";
            if (type.uint8 != null) return nullable ? "byte?" : "byte";
            if (type.@void != null) return "global::CapnProto.Void";

            ulong typeid = 0;
            if (type.@interface != null)
                typeid = type.@interface.typeId;
            else if (type.@struct != null)
                typeid = type.@struct.typeId;
            else if (type.@enum != null)
                typeid = type.@enum.typeId;
            Schema.Node node;
            if (typeid != 0 && (node = Lookup(typeid)) != null)
            {
                if(nullable)
                {
                    if (node.@enum != null) return FullyQualifiedName(node) + "?";
                    if (node.@struct != null && node.@struct.isGroup) return FullyQualifiedName(node) + "?";
                }
                return FullyQualifiedName(node);
            }

            if (type.list != null)
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
            if (node == null || node.@enum == null || node.@enum.enumerants == null) return;
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
            }
            if (union.Count != 1) Write(")");
            return this;
        }
        public override CodeWriter WriteFieldAccessor(Schema.Node parent, Schema.Field field, Stack<UnionStub> union)
        {
            string fieldOwner = parent.@struct.isGroup ? "this.parent" : "this";

            if (field.slot.type == null)
            {
                return WriteLine().Write("#warning no type for: " + field.name);
            }
            if (field.ordinal != null && field.ordinal.@implicit == null)
            {
                WriteLine().Write("[global::CapnProto.FieldAttribute(").Write(field.ordinal.@explicit).Write(")]");
            }
            var slot = field.slot;
            var type = slot.type;
            BeginProperty(field.slot.type, field.name, union.Count != 0 && type.@struct == null);
            WriteLine().Write("get");
            Indent();

            

            var len = type.GetFieldLength();

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
                var e = type.@struct == null ? null : Lookup(type.@struct.typeId);
                if (e != null && e.@struct != null && e.@struct.isGroup)
                {
                    //if(union.Count != 0)
                    //{
                    //    WriteLine().Write("if (");
                    //    WriteUnionTest(fieldOwner, union).Write(")");
                    //    Indent();
                    //}
                    WriteLine().Write("return new ").Write(FullyQualifiedName(e)).Write("(").Write(fieldOwner).Write(");");
                    //if(union.Count != 0)
                    //{
                    //    Outdent();
                    //    WriteLine().Write("return null;");
                    //}
                }
                else
                {
                    WriteLine().Write("return ");
                    if (union.Count != 0) WriteUnionTest(fieldOwner, union).Write(" ? ");
                    Write("(").Write(Format(type)).Write(")").Write(fieldOwner).Write("." + PointerPrefix).Write(slot.offset);
                    if (union.Count != 0) Write(" : null");
                    Write(";");
                }
            }
            else
            {
                int byteInData = checked((int)slot.offset * len), byteInWord = byteInData % 64;

                string fieldName = DataPrefix + (byteInData / 64);
#warning move this to a switch when available

                if (union.Count != 0)
                {
                    WriteLine().Write("if (");
                    WriteUnionTest(fieldOwner, union).Write(")");
                    Indent();
                }
                if (type.@bool != null)
                {
                    ulong mask = ((ulong)1) << byteInWord;
                    WriteLine().Write("return (").Write(fieldOwner).Write(".").Write(fieldName).Write(" & ").Write(mask).Write(") != 0;");
                    
                }
                else if ((type.int8 ?? type.int16 ?? type.int32 ?? type.int64
                  ?? type.uint8 ?? type.uint16 ?? type.uint32 ?? type.uint64) != null)
                {
                    WriteLine().Write("return unchecked((").Write(type).Write(")");
                    if (byteInWord == 0) Write(fieldOwner).Write(".").Write(fieldName);
                    else Write("(").Write(fieldOwner).Write(".").Write(fieldName).Write(" >> ").Write(byteInWord).Write(")");
                    Write(");");
                }
                else if (type.@enum != null && len == 16)
                {
                    var e = Lookup(type.@enum.typeId);
                    if (e == null || e.@enum == null || e.@enum.enumerants == null)
                    {
                        WriteLine().Write("#error enum not found: ").Write(type.@enum.typeId);
                    }
                    else
                    {
                        // all enums are Int16; so 4 
                        ulong mask = ((ulong)0xFFFF) << byteInWord;

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
                }
                else if ((type.float32 ?? type.float64) != null)
                {
                    WriteLine().Write(typeof(ulong)).Write(" tmp = ").Write(fieldOwner).Write(".").Write(fieldName);
                    if (byteInWord != 0) Write(" >> ").Write(byteInWord);
                    Write(";").WriteLine().Write("return *((").Write(type).Write("*)(&tmp));");
                }
                else
                {
                    WriteLine().Write("throw new global::System.NotImplementedException();");
                }
                if(union.Count != 0)
                {
                    Outdent();
                    WriteLine().Write("return null;");
                }
            }
            Outdent();
            return Outdent();
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
            while (parent != null && parent.@struct != null && parent.@struct.isGroup)
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
            foreach (var field in node.@struct.fields)
            {
                WriteFieldAccessor(node, field, union);
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
            foreach (var field in @struct.fields)
            {
                if (field.discriminantValue != ushort.MaxValue)
                    WriteLine().Write(Escape(field.name)).Write(" = ").Write(field.discriminantValue).Write(",");
            }
            Outdent();

            WriteLine().Write("public ").Write(FullyQualifiedName(node)).Write(".Unions Union");
            Indent();
            WriteLine().Write("get");
            Indent();
            int wordIndex = (int)@struct.discriminantOffset / 64, byteInWord = (int)@struct.discriminantOffset % 64;
            WriteLine().Write("return (").Write(FullyQualifiedName(node)).Write(".Unions)((").Write(@struct.isGroup ? "this.parent" : "this").Write(".")
                .Write(DataPrefix).Write(wordIndex);
            if (byteInWord != 0) Write(" >> ").Write(byteInWord);
            Write(") & 0xFFFF);");

            Outdent();
            return Outdent();
        }
    }
}
