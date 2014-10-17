using System.IO;
using System;
namespace CapnProto
{
    public abstract class CodeWriter
    {
        private TextWriter destination;
        public CodeWriter(TextWriter destination)
        {
            this.destination = destination;
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
    }
}
