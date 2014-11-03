using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace CapnProto
{
    public abstract class CodeWriter
    {

        public const string PrivatePrefix = "ѧ_";

        private TextWriter destination;
        Dictionary<ulong, Schema.Node> map = new Dictionary<ulong, Schema.Node>();
        public CodeWriter(TextWriter destination, List<Schema.Node> nodes, string @namespace, string serializer)
        {
            this.destination = destination;
            this.@namespace = @namespace;
            this.serializer = serializer;
            foreach (var node in nodes)
            {
                if (node.id != 0) map[node.id] = node;
            }
        }
        protected Schema.Node FindParent(Schema.Node node)
        {
            if (node == null || node.id == 0) return null;
            foreach (var pair in map)
            {
                var nested = pair.Value.nestedNodes;
                if (nested != null)
                {
                    foreach (var x in nested)
                        if (x.id == node.id) return pair.Value;
                }
            }
            return null;
        }
        public abstract CodeWriter WriteError(string message);
        public Schema.Node Lookup(ulong? id)
        {
            return id == null ? null : Lookup(id.Value);
        }
        public Schema.Node Lookup(ulong id)
        {
            Schema.Node node;
            return map.TryGetValue(id, out node) ? node : null;
        }
        public virtual CodeWriter Write(string value)
        {
            destination.Write(value);
            return this;
        }
        public virtual CodeWriter WriteLine(bool indent = true)
        {
            destination.WriteLine();
            return this;
        }

        public virtual CodeWriter BeginFile() { return this; }
        public virtual CodeWriter EndFile() { return this; }
        public abstract CodeWriter BeginNamespace(string name);
        public abstract CodeWriter EndNamespace();
        public abstract CodeWriter BeginClass(Schema.Node node);
        public abstract CodeWriter WriteLittleEndianCheck(Schema.Node node);
        public abstract CodeWriter BeginClass(bool @public, bool @internal, string name, Type baseType);
        public abstract CodeWriter EndClass();

        public abstract CodeWriter DeclareField(string name, Type type);

        public abstract CodeWriter BeginOverride(System.Reflection.MethodInfo method);

        public abstract CodeWriter WriteSerializerTest(string field, Schema.Node node, string serializer);

        public abstract CodeWriter CallBase(System.Reflection.MethodInfo method);

        public abstract CodeWriter EndOverride();

        public abstract CodeWriter Write(Type type);

        public abstract CodeWriter WriteCustomSerializerClass(Schema.Node node, string typeName, string methodName);

        public abstract CodeWriter EndMethod();

        public abstract CodeWriter WriteCustomReaderMethod(Schema.Node node);

        public abstract CodeWriter WriteField(Schema.Field field);

        public abstract string Format(Schema.Type type, bool nullable = false);

        public virtual CodeWriter Write(uint value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(uint? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(ulong? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(int value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(ulong value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(Schema.Type type)
        {
            return Write(Format(type));
        }

        public abstract void WriteEnum(Schema.Node node);
        public abstract void WriteConst(Schema.Node node);

        public virtual CodeWriter DeclareFields(string prefix, int count, Type type)
        {
            for (int i = 0; i < count; i++)
            {
                DeclareField(prefix + i, type);
            }
            return this;
        }

        public virtual CodeWriter DeclareFields(List<string> names, Type type)
        {
            foreach (var name in names)
            {
                DeclareField(name, type);
            }
            return this;
        }

        readonly string @namespace, serializer;
        public string Namespace { get { return @namespace; } }
        public string Serializer { get { return serializer; } }

        public abstract CodeWriter WriteFieldAccessor(Schema.Node parent, Schema.Field field, Stack<UnionStub> union);

        public abstract CodeWriter WriteGroupAccessor(Schema.Node parent, Schema.Node child, string name, bool extraNullable);

        public abstract CodeWriter DeclareFields(int bodyWords, int pointers);

        public abstract CodeWriter WriteGroup(Schema.Node node, Stack<UnionStub> union);

        public abstract CodeWriter WriteDiscriminant(Schema.Node node, Stack<UnionStub> union);

        public virtual CodeWriter WriteNestedTypes(Schema.Node node, Stack<UnionStub> union)
        {
            var children = node.nestedNodes;
            if (children != null)
            {
                foreach (var child in children)
                {
                    var found = Lookup(child.id);
                    if (found != null)
                    {
                        WriteNode(found, union);
                    }
                    else WriteError("not found: " + child.id + " / " + child.name);
                }
            }
            return this;
        }
        public virtual CodeWriter WriteNode(Schema.Node node, Stack<UnionStub> union)
        {
            if (node != null)
            {
                if (node.@struct != null) WriteStruct(node, union);
                if (node.@enum != null) WriteEnum(node);
                if (node.@const != null) WriteConst(node);
            }
            return this;
     
        }
        public void WriteStruct(Schema.Node node, Stack<UnionStub> union)
        {
            var @struct = node.@struct;

            if (@struct == null || @struct.isGroup) return;

            if (@struct.isGroup)
            {
                //WriteGroup(node, union);
            }
            else
            {
                // the nested type does not inherit the unions from the caller
                if (union.Count != 0)
                {
                    union = new Stack<UnionStub>();
                }
                BeginClass(node).WriteLittleEndianCheck(node);

                var fields = @struct.fields;

                int bodyWords = 0, pointerWords = 0;
                Schema.CodeGeneratorRequest.ComputeSpace(this, node, ref bodyWords, ref pointerWords);
                HashSet<ulong> nestedDone = null;
                foreach (var field in fields)
                {
                    if (field.discriminantValue != ushort.MaxValue)
                    {
                        // write with union-based restructions
                        union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                        WriteFieldAccessor(node, field, union);

                        if(field.slot != null && field.slot.type != null && field.slot.type.@struct != null)
                        {
                            if (nestedDone == null) nestedDone = new HashSet<ulong>();
                            if (nestedDone.Add(field.slot.type.@struct.typeId))
                            {
                                var found = Lookup(field.slot.type.@struct.typeId);
                                if (found != null && found.@struct != null && found.@struct.isGroup)
                                {
                                    WriteGroup(found, union);
                                }
                            }
                        }
                        union.Pop();
                    }
                    else
                    {
                        // just write the damned field
                        WriteFieldAccessor(node, field, union);
                    }
                }
                if (node.nestedNodes != null)
                {
                    foreach(var nestedNode in node.nestedNodes)
                    {
                        if(nestedDone == null || !nestedDone.Contains(nestedNode.id))
                        {
                            var found = Lookup(nestedNode.id);
                            if (found != null && found.@struct != null && found.@struct.isGroup)
                            {
                                WriteGroup(found, union);
                                WriteGroupAccessor(node, found, "get_" + found.displayName, false);
                            }
                        }
                    }
                }
                if (@struct.discriminantCount != 0)
                {
                    WriteDiscriminant(node, union);
                }

                DeclareFields(bodyWords, pointerWords);

                WriteNestedTypes(node, union);
                EndClass();
            }


        }
    }
}
