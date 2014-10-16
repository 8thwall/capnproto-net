using System;
using System.IO;

namespace CapnProto
{
    public class CapnProtoSerializer
    {
        private readonly ITypeSerializer serializer;

        protected ITypeSerializer Serializer { get { return serializer; } }
        public CapnProtoSerializer(Type type)
        {
            serializer = TypeModel.Default.GetSerializer(type);
        }

        public object Deserialize(CapnProtoReader source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Serializer.Deserialize(source);
        }
        public object Deserialize(Stream source)
        {
            if (source == null) throw new ArgumentNullException("source");
            using(var reader = CapnProtoReader.Create(source, null))
            {
                return Serializer.Deserialize(reader);
            }
        }
    }

    public class CapnProtoSerializer<T> : CapnProtoSerializer
    {
        public CapnProtoSerializer() : base(typeof(T)) { }
        protected new ITypeSerializer<T> Serializer { get { return (ITypeSerializer<T>)base.Serializer; } }
        public new T Deserialize(CapnProtoReader source)
        {
            if (source == null) throw new ArgumentNullException("source");
            return Serializer.Deserialize(source);
        }
        public new T Deserialize(Stream source)
        {
            if (source == null) throw new ArgumentNullException("source");
            using (var reader = CapnProtoReader.Create(source, null))
            {
                return Serializer.Deserialize(reader);
            }
        }
    }
}
