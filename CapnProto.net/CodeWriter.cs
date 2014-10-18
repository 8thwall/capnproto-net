using System.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
namespace CapnProto
{
    public abstract class CodeWriter
    {
        private TextWriter destination;
        Dictionary<ulong, Schema.Node> map = new Dictionary<ulong, Schema.Node>();
        public CodeWriter(TextWriter destination, List<Schema.Node> nodes)
        {
            this.destination = destination;
            foreach(var node in nodes)
            {
                if (node.id != 0) map[node.id] = node;
            }
        }
        public abstract CodeWriter WriteError(string message);
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
        public virtual CodeWriter WriteLine()
        {
            destination.WriteLine();
            return this;
        }

        public virtual CodeWriter BeginFile() { return this; }
        public virtual CodeWriter EndFile() { return this; }
        public abstract CodeWriter BeginNamespace(string name);
        public abstract CodeWriter EndNamespace();
        public abstract CodeWriter BeginClass(Schema.Node node);
        public abstract CodeWriter BeginClass(bool @public, string name, Type baseType);
        public abstract CodeWriter EndClass();

        public abstract CodeWriter DeclareField(string name, Type type);

        public abstract CodeWriter BeginOverride(System.Reflection.MethodInfo method);

        public abstract CodeWriter WriteSerializerTest(string field, Schema.Node node, string serializer);

        public abstract CodeWriter CallBase(System.Reflection.MethodInfo method);

        public abstract CodeWriter EndOverride();

        public abstract CodeWriter Write(Type type);

        public abstract CodeWriter WriteCustomSerializerClass(Schema.Node node, string baseType, string typeName, string methodName);

        public abstract CodeWriter EndMethod();

        public abstract CodeWriter WriteCustomReaderMethod(Schema.Node node, string name);

        public abstract CodeWriter WriteField(Schema.Field field);

        public abstract string Format(Schema.Type type);

        public virtual CodeWriter Write(uint value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }
        public virtual CodeWriter Write(int value)
        {
            return Write(value.ToString(CultureInfo.InvariantCulture));
        }

        public virtual CodeWriter Write(Schema.Type type)
        {
            return Write(Format(type));
        }

        public abstract void WriteEnum(CodeWriter writer, Schema.Node node);

        public virtual CodeWriter DeclareFields(string prefix, int count, Type type)
        {
            for(int i = 0 ; i < count ; i++)
            {
                DeclareField(prefix + i, type);
            }
            return this;
        }
    }
}
