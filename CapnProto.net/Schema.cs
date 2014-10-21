using System;
using System.Collections.Generic;
using System.Reflection;
namespace CapnProto
{

    public static partial class Schema
    {
        partial class CodeGeneratorRequest
        {
            public static CodeGeneratorRequest Create<T>()
            {
                return Create(typeof(T));
            }
            public static CodeGeneratorRequest Create(global::System.Type type)
            {
                if (type == null) throw new ArgumentNullException();

                Dictionary<System.Type, Node> map = new Dictionary<System.Type, Node>();
                Queue<System.Type> pending = new Queue<System.Type>();
                pending.Enqueue(type);

                var nodes = new List<Node>(); // in discovery order

                // first just get everything without configuring all the nodes; this avoids a number of cyclic issues
                while (pending.Count != 0)
                {
                    System.Type next = pending.Dequeue();
                    if (next != null && !map.ContainsKey(next))
                    {
                        ulong id = 0;
                        string name = next.Name;
                        IdAttribute idAttrib = (IdAttribute)Attribute.GetCustomAttribute(next, typeof(IdAttribute));
                        if (idAttrib != null)
                        {
                            id = idAttrib.Id;
                            if (!string.IsNullOrWhiteSpace(idAttrib.Name)) name = idAttrib.Name;
                        }
                        var node = new Node { id = id, displayName = name };
                        map.Add(next, node);
                        nodes.Add(node);
                        Cascade(next, pending);
                    }
                }

                foreach (var pair in map)
                {
                    ConfigureNode(pair.Key, pair.Value, map);
                }

                return new CodeGeneratorRequest
                {
                    nodes = nodes,
                    requestedFiles = new List<RequestedFile>()
                };
            }

            private static bool Include(System.Type type)
            {
                if (type == null) return false;
                return Attribute.IsDefined(type, typeof(GroupAttribute)) || Attribute.IsDefined(type, typeof(IdAttribute));
            }
            static System.Type GetInnermostElementType(System.Type type)
            {
                System.Type last = type;
                while (type != null)
                {
                    last = type;
                    type = GetElementType(type);
                }
                return last;
            }

            static System.Type GetElementType(System.Type type)
            {
                if (type == null) return null;

                // T[]
                if (type.IsArray) return type.GetElementType();

                // look for a **single** IList<T>
                int count = 0;
                System.Type tmp = null;
                foreach (var iType in type.GetInterfaces())
                {
                    if (iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        if (++count == 2) break;
                        tmp = iType.GetGenericArguments()[0];
                    }
                }
                if (count == 1) return tmp;


                // nothing obvious; fail
                return null;
            }
            private static void Cascade(System.Type next, Queue<System.Type> pending)
            {
                foreach (var type in next.GetNestedTypes())
                {
                    if (Include(type)) pending.Enqueue(type);
                }
                foreach (var field in next.GetFields())
                {
                    if (Include(field.FieldType)) pending.Enqueue(field.FieldType);
                    else
                    {
                        var elementType = GetInnermostElementType(field.FieldType);
                        if (Include(elementType)) pending.Enqueue(elementType);
                    }
                }
                foreach (var prop in next.GetProperties())
                {
                    if (Include(prop.PropertyType)) pending.Enqueue(prop.PropertyType);
                    else
                    {
                        var elementType = GetInnermostElementType(prop.PropertyType);
                        if (Include(elementType)) pending.Enqueue(elementType);
                    }
                }
            }


            private static void ConfigureNode(System.Type type, Node node, Dictionary<System.Type, Node> map)
            {

                List<NestedNode> nestedNodes = new List<NestedNode>();
                List<Field> fields = new List<Field>();

                node.nestedNodes = nestedNodes;
                if (type.IsEnum)
                {
                    var eg = node.@enum = new Node.enumGroup { enumerants = new List<Enumerant>()};
                    foreach(var field in type.GetFields())
                    {
                        var fa = (FieldAttribute)Attribute.GetCustomAttribute(field, typeof(FieldAttribute));
                        if (fa == null) continue;
                        eg.enumerants.Add(new Enumerant { codeOrder = checked((ushort)fa.Number), name = field.Name });
                    }
                }
                else
                {
                    node.@struct = new Node.structGroup
                        {
                            fields = fields,
                            isGroup = Attribute.IsDefined(type, typeof(GroupAttribute))
                        };

                }

                var nestedTypes = type.GetNestedTypes();
                foreach (var nestedType in nestedTypes)
                {
                    Node nested;
                    if (map.TryGetValue(nestedType, out nested))
                    {
                        nestedNodes.Add(new NestedNode
                        {
                            id = nested.id,
                            name = nested.displayName
                        });
                    }

                }
                ushort discCount = 0;
                uint discOffset = 0;
                foreach (var field in type.GetFields())
                {
                    var member = CreateField(field, ref discCount, ref discOffset);
                    if (member != null) fields.Add(member);
                }
                foreach (var prop in type.GetProperties())
                {
                    var member = CreateField(prop, ref discCount, ref discOffset);
                    if (member != null) fields.Add(member);
                }
                if (discCount != 0 && node.@struct != null)
                {
                    node.@struct.discriminantCount = discCount;
                    node.@struct.discriminantOffset = discOffset;
                }
            }
            static Field CreateField(MemberInfo member, ref ushort discCount, ref uint discOffset)
            {
                if (member == null) return null;
                System.Type type;
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        type = ((FieldInfo)member).FieldType; break;
                    case MemberTypes.Property:
                        type = ((PropertyInfo)member).PropertyType; break;
                    default:
                        return null;
                }
                var fa = (FieldAttribute)Attribute.GetCustomAttribute(member, typeof(FieldAttribute));
                var ua = (UnionAttribute)Attribute.GetCustomAttribute(member, typeof(UnionAttribute));
                if (fa == null && ua == null) return null;

                Field field = new Field();
                var slot = new Schema.Field.slotGroup();
                int len;
                slot.type = GetSchemaType(type, out len);
                field.slot = slot;
                if (fa != null)
                {
                    if (fa.Start >= 0)
                    {
                        slot.offset = len == 0 ? 0 : checked((uint)(fa.Start / len));
                    }
                    else if (fa.Pointer >= 0)
                    {
                        slot.offset = checked((uint)fa.Pointer);
                    }
                    var ordinal = new Schema.Field.ordinalGroup();
                    if (fa.Number >= 0)
                    {
                        ordinal.@explicit = checked((ushort)fa.Number);
                    }
                    else
                    {
                        ordinal.@implicit = Void.Value;
                    }
                    field.ordinal = ordinal;
                }

                if (ua != null)
                {
                    discCount++;
                    if(ua.Tag == 0)
                    {
                        discOffset = checked((ushort)ua.Start);
                    }
                    field.discriminantValue = checked((ushort)ua.Tag);
                }
                else
                {
                    field.discriminantValue = ushort.MaxValue;
                }
                field.name = member.Name;

                return field;
            }
            private static Type GetSchemaType(System.Type type, out int len)
            {
                if (type == null)
                {
                    len = 0;
                    return null;
                }

                // objects and enums
                var idAttrib = (IdAttribute)Attribute.GetCustomAttribute(type, typeof(IdAttribute));
                if (idAttrib != null)
                {
                    if (type.IsEnum)
                    {
                        len = 16;
                        return new Type { @enum = new Type.enumGroup { typeId = idAttrib.Id } };
                    }
                    if (type.IsClass || type.IsValueType)
                    {
                        len = Type.LEN_POINTER;
                        return new Type { @struct = new Type.structGroup { typeId = idAttrib.Id } };
                    }
                }

                if (type == typeof(byte[]))
                {
                    len = 0;
                    return new Type { data = Void.Value };
                } else if (type == typeof(Void))
                {
                    len = 0;
                    return new Type { @void = Void.Value };
                }
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Empty: len = 0; return null;
                    case TypeCode.Boolean: len = 8; return new Type { @bool = Void.Value };
                    case TypeCode.Byte: len = 16; return new Type { uint8 = Void.Value };
                    case TypeCode.SByte: len = 8; return new Type { int8 = Void.Value };
                    case TypeCode.Int16: len = 16; return new Type { int16 = Void.Value };
                    case TypeCode.UInt16: len = 16; return new Type { uint16 = Void.Value };
                    case TypeCode.Int32: len = 32; return new Type { int32 = Void.Value };
                    case TypeCode.UInt32: len = 32; return new Type { uint32 = Void.Value };
                    case TypeCode.Int64: len = 64; return new Type { int64 = Void.Value };
                    case TypeCode.UInt64: len = 64; return new Type { uint64 = Void.Value };
                    case TypeCode.Char: len = 16; return new Type { uint16 = Void.Value };
                    case TypeCode.DBNull: len = 0; return new Type { @void = Void.Value };
                    case TypeCode.Single: len = 32; return new Type { float32 = Void.Value };
                    case TypeCode.Double: len = 64; return new Type { float64 = Void.Value };
                    case TypeCode.String: len = Type.LEN_POINTER; return new Type { text = Void.Value };
                }

                // lists (note this includes recursive)
                var elType = GetSchemaType(GetElementType(type), out len);
                if (elType != null)
                {
                    len = Type.LEN_POINTER;
                    return new Type { list = new Type.listGroup { elementType = elType } };
                }
                len = 0;
                return null;
            }
            internal const string BaseTypeName = CodeWriter.PrivatePrefix + "SerializerBase";
            public void GenerateCustomModel(CodeWriter writer)
            {
                writer.BeginFile().BeginNamespace(writer.Namespace);

                var nested = new HashSet<ulong>();
                foreach (var node in this.nodes)
                {
                    var children = node.nestedNodes;
                    if (children != null)
                    {
                        foreach (var child in children)
                            if (child.id != 0) nested.Add(child.id);
                    }
                }
                foreach (var node in this.nodes)
                {
                    var union = new Stack<UnionStub>();
                    if (node.id != 0 && !nested.Contains(node.id))
                    {
                        writer.WriteNode(node, union);
                    }
                }

                writer.BeginClass(true, false, writer.Serializer, typeof(TypeModel));

                List<Node> generateSerializers = new List<Node>(this.nodes.Count);
                List<string> fieldNames = new List<string>();
                var uniques = new HashSet<ulong>();
                foreach (var node in this.nodes)
                {
                    if(!uniques.Add(node.id))
                    {
                        throw new InvalidOperationException("Duplicate id: " + node.id + " / " + node.UniqueName() + " on " + node.displayName);
                    }
                    if (node.@struct != null && !node.@struct.isGroup)
                    {
                        generateSerializers.Add(node);
                        fieldNames.Add(CodeWriter.PrivatePrefix + "f_" + node.UniqueName());
                    }
                }
                
                writer.DeclareFields(fieldNames, typeof(ITypeSerializer));
                
                var method = typeof(TypeModel).GetMethod("GetSerializer", getSerializerSignature);
                writer.BeginOverride(method);
                
                foreach (var node in generateSerializers)
                {
                    writer.WriteSerializerTest(CodeWriter.PrivatePrefix + "f_" + node.UniqueName(), node, CodeWriter.PrivatePrefix + "s_" + node.UniqueName());
                }
                writer.CallBase(method);
                writer.EndOverride();
                
                foreach (var node in generateSerializers)
                {
                    writer.WriteCustomSerializerClass(node, CodeWriter.PrivatePrefix + "s_" + node.UniqueName(), node.CustomSerializerName());
                }

                writer.BeginClass(false, true, BaseTypeName, null);
                foreach (var node in generateSerializers)
                {
                    writer.WriteCustomReaderMethod(node);
                }
                writer.EndClass();

                writer.EndClass();
                writer.EndNamespace().EndFile();


            }

            
            static readonly System.Type[] getSerializerSignature = new[] { typeof(System.Type) };

            internal static void ComputeSpace(CodeWriter writer, Node node, ref int bodyWords, ref int pointerWords)
            {
                int bodyEnd = 0;
                if(node.@struct.discriminantCount != 0)
                {
                    bodyEnd = (int)(node.@struct.discriminantOffset + 16);
                }
                foreach (var field in node.@struct.@fields)
                {
                    var slot = field.slot;
                    if (slot == null || slot.type == null) continue;
                    int len = slot.type.GetFieldLength();

                    var relatedType = slot.type.@struct == null ? null : writer.Lookup(slot.type.@struct.typeId);
                    if (relatedType != null && relatedType.@struct != null && relatedType.@struct.isGroup)
                    {
                        ComputeSpace(writer, relatedType, ref bodyWords, ref pointerWords);
                    }
                    else if (len == 0) { }
                    else if (len == Type.LEN_POINTER)
                    {
                        int end = checked((int)slot.offset + 1);
                        if (end > pointerWords) pointerWords = end;
                    }
                    else
                    {
                        int end = checked(len * (int)(slot.offset + 1));
                        if (end > bodyEnd) bodyEnd = end;
                    }
                }
                var localBodyWords = (bodyEnd + 63) / 64;
                if (localBodyWords > bodyWords) bodyWords = localBodyWords;
            }

            

        }



    }
}
