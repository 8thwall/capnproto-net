using System;
using System.IO;

namespace CapnProto
{
    public static class TypeSerializer
    {
        public static object Deserialize(this ITypeSerializer serializer, Stream source)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (source == null) throw new ArgumentNullException("source");
            using(var reader = CapnProtoReader.Create(source, null))
            {
                return serializer.Deserialize(reader);
            }
        }
        public static T Deserialize<T>(this ITypeSerializer<T> serializer, Stream source)
        {
            if (serializer == null) throw new ArgumentNullException("serializer");
            if (source == null) throw new ArgumentNullException("source");
            using (var reader = CapnProtoReader.Create(source, null))
            {
                return serializer.Deserialize(reader);
            }
        }
    }
}
