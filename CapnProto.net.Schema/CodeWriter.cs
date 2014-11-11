using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using CapnProto.Schema;
using System.Linq;
namespace CapnProto
{
    public abstract class CodeWriter
    {

        public abstract bool NeedsSerializer { get; }
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
            if (!node || node.id == 0) return default(Node);
            if (node.scopeId != 0 && node.scopeId != node.id)
            {
                var tmp = Lookup(node.scopeId);
                if (tmp) return tmp;
            }
            foreach (var pair in map)
            {
                var nested = pair.Value.nestedNodes;
                if (nested)
                {
                    foreach (var x in nested)
                        if (x.id == node.id) return pair.Value;
                }
            }
            return default(Node);
        }
        public abstract CodeWriter WriteError(string message);
        public abstract CodeWriter WriteWarning(string message);
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
        public abstract CodeWriter BeginClass(bool @public, bool @internal, string name, System.Type baseType);
        public abstract CodeWriter EndClass();

        public abstract CodeWriter DeclareField(string name, System.Type type);

        public abstract CodeWriter BeginOverride(System.Reflection.MethodInfo method);

        public abstract CodeWriter WriteSerializerTest(string field, Schema.Node node, string serializer);

        public abstract CodeWriter CallBase(System.Reflection.MethodInfo method);

        public abstract CodeWriter EndOverride();

        public abstract CodeWriter Write(System.Type type);

        public abstract CodeWriter WriteCustomSerializerClass(Schema.Node node, string typeName, string methodName);

        public abstract CodeWriter EndMethod();

        public abstract CodeWriter WriteCustomReaderMethod(Schema.Node node);

        public abstract CodeWriter WriteField(Schema.Field field);

        public abstract string Format(Schema.Type type, bool nullable = false);

        public virtual CodeWriter WriteLiteral(sbyte value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(byte value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(short value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(ushort value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(int value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(uint value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(long value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(ulong value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(float value) { return Write(value); }
        public virtual CodeWriter WriteLiteral(double value) { return Write(value); }
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
        public virtual CodeWriter Write(int? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(byte value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(byte? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(sbyte value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(sbyte? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(short value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(short? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(ushort value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(ushort? value)
        {
            return Write(value.Value);
        }
        public virtual CodeWriter Write(ulong value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(long value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public abstract CodeWriter Write(bool value);

        public virtual CodeWriter Write(float value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(double value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public abstract CodeWriter WriteLiteral(string value);

        public virtual CodeWriter Write(Schema.Type type, Value value)
        {
            if (!value) return Write("null");
            switch (value.Union)
            {
                case Value.Unions.@bool: return Write(value.@bool);
                case Value.Unions.float32: return Write(value.float32);
                case Value.Unions.float64: return Write(value.float64);
                case Value.Unions.int8: return Write(value.int8);
                case Value.Unions.uint8: return Write(value.uint8);
                case Value.Unions.int16: return Write(value.int16);
                case Value.Unions.uint16: return Write(value.uint16);
                case Value.Unions.int32: return Write(value.int32);
                case Value.Unions.uint32: return Write(value.uint32);
                case Value.Unions.int64: return Write(value.int64);
                case Value.Unions.uint64: return Write(value.uint64);
                case Value.Unions.text: return WriteLiteral(value.text.ToString());
                case Value.Unions.@enum: return WriteEnumLiteral(type, value.@enum);
            }
            throw new NotSupportedException("Cannot write value: " + value.Union);
        }

        public abstract CodeWriter WriteEnumLiteral(Schema.Type type, ushort value);

        public virtual CodeWriter Write(Schema.Type.Unions type)
        {
            return Write(type.ToString());
        }
        public virtual CodeWriter Write(Schema.Type type)
        {
            return Write(Format(type));
        }

        public abstract void WriteEnum(Schema.Node node);
        public abstract CodeWriter WriteConst(Schema.Node node);

        public virtual CodeWriter DeclareFields(string prefix, int count, System.Type type)
        {
            for (int i = 0; i < count; i++)
            {
                DeclareField(prefix + i, type);
            }
            return this;
        }

        public virtual CodeWriter DeclareFields(List<string> names, System.Type type)
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

        protected internal virtual string LocalName(string name, bool escape = true)
        {
            if (name == null) return "";
            int idx = name.LastIndexOfAny(nameTokens);
            if (idx >= 0) name = name.Substring(idx + 1);
            return name;
        }
        static readonly char[] nameTokens = { '.', '/', '\\', ':' };

        public abstract CodeWriter WriteGroupAccessor(Schema.Node parent, Schema.Node child, string name, bool extraNullable);

        public abstract CodeWriter DeclareFields(int bodyWords, int pointers);

        public abstract CodeWriter WriteGroup(Schema.Node node, Stack<UnionStub> union);

        public abstract CodeWriter WriteDiscriminant(Schema.Node node, Stack<UnionStub> union);

        public virtual CodeWriter WriteNestedTypes(Schema.Node node, Stack<UnionStub> union)
        {
            var children = node.nestedNodes;
            var seen = new HashSet<ulong>();
            if (children)
            {
                foreach (var child in children)
                {
                    if (seen.Add(child.id))
                    {
                        var found = Lookup(child.id);
                        if (found)
                        {
                            WriteNode(found, union);
                        }
                        else WriteWarning("not found: " + child.id + " / " + child.name);
                    }
                }
            }
            return this;
        }
        public IEnumerable<Node> NodesByParentScope(ulong parentId)
        {
            if (parentId != 0)
            {
                foreach (var child in map.Values)
                {
                    if (child.scopeId == parentId) yield return child;
                }
            }
        }
        public virtual CodeWriter WriteNode(Schema.Node node, Stack<UnionStub> union)
        {
            if (node)
            {
                switch (node.Union)
                {
                    case Schema.Node.Unions.@struct:
                        WriteStruct(node, union);
                        break;
                    case Schema.Node.Unions.@enum:
                        WriteEnum(node);
                        break;
                    case Schema.Node.Unions.@const:
                        WriteConst(node);
                        break;
                }
            }
            return this;

        }
        public void WriteStruct(Schema.Node node, Stack<UnionStub> union)
        {
            if (node.IsGroup()) return;
            if (node.Union != Schema.Node.Unions.@struct) return;
            var @struct = node.@struct;
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

            if (fields != null)
            {
                foreach (var field in fields.OrderBy(x => x.codeOrder).ThenBy(x => x.name))
                {
                    bool pushed = false;
                    if (field.discriminantValue != Field.noDiscriminant)
                    {
                        // write with union-based restructions
                        union.Push(new UnionStub(node.@struct.discriminantOffset, field.discriminantValue));
                        pushed = true;
                    }

                    WriteFieldAccessor(node, field, union);

                    // declare the struct too, if we need to - noting that it includes union-context
                    Node child = default(Node);
                    switch (field.Union)
                    {
                        case Field.Unions.group:
                            child = Lookup(field.group.typeId);
                            break;
                        case Field.Unions.slot:
                            if (field.slot.type.Union == Schema.Type.Unions.@struct)
                                child = Lookup(field.slot.type.@struct.typeId);
                            break;
                    }
                    if (child && child.IsGroup())
                    {
                        if (nestedDone == null) nestedDone = new HashSet<ulong>();
                        if (nestedDone.Add(child.id))
                        {
                            WriteGroup(child, union);
                        }
                    }

                    if (pushed) union.Pop();
                }
            }
            //if (node.nestedNodes != null)
            //{
            //    foreach (var nestedNode in node.nestedNodes)
            //    {
            //        if (nestedDone == null || !nestedDone.Contains(nestedNode.id))
            //        {
            //            var found = Lookup(nestedNode.id);
            //            if (found != null && found.IsGroup())
            //            {
            //                WriteGroup(found, union);
            //                WriteGroupAccessor(node, found, LocalName(found.displayName, false), false);
            //            }
            //        }
            //    }
            //}
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
