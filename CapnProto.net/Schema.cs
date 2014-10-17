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
                    nodes.Add(pair.Value);
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
                node.@struct = new Node.structGroup
                    {
                        fields = fields,
                        isGroup = Attribute.IsDefined(type, typeof(GroupAttribute))
                    };


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
                foreach (var field in type.GetFields())
                {
                    var member = CreateField(field);
                    if (member != null) fields.Add(member);
                }
                foreach (var prop in type.GetProperties())
                {
                    var member = CreateField(prop);
                    if (member != null) fields.Add(member);
                }
            }
            static Field CreateField(MemberInfo member)
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
                slot.type = GetSchemaType(type);
                field.slot = slot;
                if (fa != null)
                {
                    if (fa.Start >= 0)
                    {
                        slot.offset = checked((uint)fa.Start);
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
                }

                if (ua != null) field.discriminantValue = checked((ushort)ua.Tag);
                field.name = member.Name;

                return field;
            }

            private static Type GetSchemaType(System.Type type)
            {
                if (type == null) return null;

                // objects and enums
                var idAttrib = (IdAttribute)Attribute.GetCustomAttribute(type, typeof(IdAttribute));
                if (idAttrib != null)
                {
                    if(type.IsEnum)
                        return new Type { @enum = new Type.enumGroup { typeId = idAttrib.Id } };
                    return new Type { @struct = new Type.structGroup { typeId = idAttrib.Id } };
                }

                if(type == typeof(byte[])) return new Type { data = Void.Value };
                switch (System.Type.GetTypeCode(type))
                {
                    case TypeCode.Empty: return null;
                    case TypeCode.Boolean: return new Type { @bool = Void.Value };
                    case TypeCode.Byte: return new Type { uint8 = Void.Value };
                    case TypeCode.SByte: return new Type { int8 = Void.Value };
                    case TypeCode.Int16: return new Type { int16 = Void.Value };
                    case TypeCode.UInt16: return new Type { uint16 = Void.Value };
                    case TypeCode.Int32: return new Type { int32 = Void.Value };
                    case TypeCode.UInt32: return new Type { uint32 = Void.Value };
                    case TypeCode.Int64: return new Type { int64 = Void.Value };
                    case TypeCode.UInt64: return new Type { uint64 = Void.Value };
                    case TypeCode.Char: return new Type { uint16 = Void.Value };
                    case TypeCode.DBNull: return new Type { @void = Void.Value };
                    case TypeCode.Single: return new Type { float32 = Void.Value };
                    case TypeCode.Double: return new Type { float64 = Void.Value };
                    case TypeCode.String: return new Type { text = Void.Value };
                }

                // lists (note this includes recursive)
                var elType = GetSchemaType(GetElementType(type));
                if (elType != null) return new Type { list = new Type.listGroup { elementType = elType } };
                
                return null;
            }



            public void GenerateCustomModel(CodeWriter writer)
            {
                const string PREFIX = "_capnp_";
                const string @namespace = "test", serializerType = "myserializer";
                writer.BeginFile().BeginNamespace(@namespace);

                foreach (var node in this.nodes)
                {
                    WriteStruct(writer, node);
                }

                writer.BeginClass(true, serializerType, typeof(TypeModel));

                List<Node> generateSerializers = new List<Node>(this.nodes.Count);

                foreach (var node in this.nodes)
                {
                    if (node.@struct != null && !node.@struct.isGroup)
                    {
                        generateSerializers.Add(node);
                    }
                }

                int i = 0;
                foreach (var node in generateSerializers)
                {
                    writer.DeclareField(PREFIX + "f_" + i, typeof(ITypeSerializer));
                    i++;
                }

                var method = typeof(TypeModel).GetMethod("GetSerializer", getSerializerSignature);
                writer.BeginOverride(method);
                i = 0;
                foreach (var node in generateSerializers)
                {
                    writer.WriteSerializerTest(PREFIX + "f_" + i, node, PREFIX + "s_" + i);
                    i++;
                }
                writer.CallBase(method);
                writer.EndOverride();

                string baseTypeName = PREFIX + "b_" + serializerType;
                i = 0;
                foreach (var node in generateSerializers)
                {
                    writer.WriteCustomSerializerClass(node, baseTypeName, PREFIX + "s_" + i, PREFIX + "r_" + i);
                    i++;
                }

                writer.BeginClass(false, baseTypeName, null);
                i = 0;
                foreach (var node in generateSerializers)
                {
                    writer.WriteCustomReaderMethod(node, PREFIX + "r_" + i);
                    i++;
                }
                writer.EndClass();

                writer.EndClass();
                writer.EndNamespace().EndFile();


            }
            static readonly System.Type[] getSerializerSignature = new[] { typeof(System.Type) };

            private void WriteStruct(CodeWriter writer, Node node)
            {
                var @struct = node.@struct;
                if (@struct == null) return;

                writer.BeginClass(node);

                writer.EndClass();
            }
        }


    }
}
