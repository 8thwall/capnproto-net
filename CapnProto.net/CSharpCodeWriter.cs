using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CapnProto
{
    public class CSharpCodeWriter : CodeWriter
    {
        public CSharpCodeWriter(TextWriter destination, List<Schema.Node> nodes, string @namespace) : base(destination, nodes, @namespace) { }
        private int indentationLevel;

        public override CodeWriter WriteLine()
        {
            base.WriteLine();
            int tmp = indentationLevel;
            while (tmp-- > 0)
            {
                Write("    ");
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
                if (node.@struct.isGroup)
                {
                    WriteLine().Write("[global::CapnProto.Group]");
                }
                if (node.id != 0)
                {
                    WriteLine().Write("[global::CapnProto.Id(").Write(node.id).Write(")]");
                }
            }
            WriteLine().Write("public partial class ").Write(LocalName(node));
            return Indent();
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

        public override CodeWriter BeginClass(bool @public, string name, Type baseType)
        {
            WriteLine();
            if (@public) Write("public ");
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
            if (type == null || type == typeof(Void)) return "void";
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

        public override CodeWriter WriteCustomSerializerClass(Schema.Node node, string baseType, string typeName, string methodName)
        {
            WriteLine().Write("class ").Write(typeName).Write(" : ").Write(baseType).Write(", global::CapnProto.ITypeSerializer<").Write(FullyQualifiedName(node)).Write(">");
            Indent();

            WriteLine().Write("object global::CapnProto.ITypeSerializer.Deserialize(global::CapnProto.CapnProtoReader reader)");
            Indent().WriteLine().Write("return Deserialize(reader);");
            EndMethod();

            WriteLine().Write("public ").Write(FullyQualifiedName(node)).Write(" Deserialize(global::CapnProto.CapnProtoReader reader)");
            Indent().WriteLine().Write("reader.ReadPreamble();").WriteLine().Write("return ").Write(methodName).Write("(1, reader, reader.ReadWord(0));");
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
        public override CodeWriter WriteCustomReaderMethod(Schema.Node node, string name)
        {
            WriteLine().Write("protected ").Write(FullyQualifiedName(node)).Write(" ").Write(name).Write("(int wordOffset, global::CapnProto.CapnProtoReader reader, ulong ptr)");

            Indent().WriteLine();
            //int maxBody = -1;
            foreach (var field in node.@struct.fields)
            {
                var slot = field.slot;
                if (slot == null || slot.type == null) continue;
                int len = slot.type.GetFieldLength();
                WriteLine().Write("// ");
                var ord = field.ordinal;
                if (ord != null && ord.@implicit == null)
                {
                    Write("@").Write(ord.@explicit).Write(" ");
                }
                Write(field.name).Write(": ");
                switch (len)
                {
                    case 0:
                        Write("nothing to do");
                        break;
                    case -1:
                        Write("pointer ").Write(slot.offset);
                        break;
                    default:
                        int start = checked(len * (int)slot.offset);
                        Write("bit ").Write(start).Write(" (word ").Write(start / 64).Write(" shift ").Write(start % 64).Write(")");
                        break;
                }
            }
            WriteLine().Write("throw new global::System.NotImplementedException();");

            return EndMethod();
        }

        public override string Format(Schema.Type type)
        {
            if (type == null) return null;
            if (type.anyPointer != null) return "object";
            if (type.@bool != null) return "bool";
            if (type.data != null) return "byte[]";
            if (type.float32 != null) return "float";
            if (type.float64 != null) return "double";
            if (type.int16 != null) return "short";
            if (type.int32 != null) return "int";
            if (type.int64 != null) return "long";
            if (type.int8 != null) return "sbyte";
            if (type.text != null) return "string";
            if (type.uint16 != null) return "ushort";
            if (type.uint32 != null) return "uint";
            if (type.uint64 != null) return "ulong";
            if (type.uint8 != null) return "byte";
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

        public override void WriteEnum(CodeWriter writer, Schema.Node node)
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
        private void BeginProperty(Schema.Type type, string name)
        {
            WriteLine().Write("public ").Write(Format(type)).Write(" ").Write(Escape(name));
            Indent();
        }
        public override CodeWriter WriteFieldAccessor(Schema.Field field)
        {
            BeginProperty(field.slot.type, field.name);
            WriteLine().Write("get");
            Indent();

            var slot = field.slot;
            var type = slot.type;

            var len = type.GetFieldLength();
            if (len == 0)
            {
                WriteLine().Write("return ").Write(typeof(Void)).Write(".Value;");
            }
            else if (len == Schema.Type.LEN_POINTER)
            {
                WriteLine().Write("return (").Write(Format(type)).Write(")" + PointerPrefix).Write(slot.offset).Write(";");
            }
            else
            {
                int byteInData = checked((int)slot.offset * len), byteInWord = byteInData % 64;

                string fieldName = DataPrefix + (byteInData / 64);
#warning move this to a switch when available
                if (type.@bool != null)
                {
                    ulong mask = ((ulong)1) << byteInWord;
                    WriteLine().Write("return (").Write(fieldName).Write(" & ").Write(mask).Write(") != 0;");
                } else if ((type.int8 ?? type.int16 ?? type.int32 ?? type.int64
                    ?? type.uint8 ?? type.uint16 ?? type.uint32 ?? type.uint64) != null )
                {
                    WriteLine().Write("return unchecked((").Write(type).Write(")");
                    if(byteInWord == 0) Write(fieldName);
                    else Write("(").Write(fieldName).Write(" >> ").Write(byteInWord).Write(")");
                    Write(");");
                }
                else
                {
                    WriteLine().Write("throw new global::System.NotImplementedException();");
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
    }
}
