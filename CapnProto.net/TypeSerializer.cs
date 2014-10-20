using System;
using System.ComponentModel;
using System.IO;

namespace CapnProto
{
    public static class TypeSerializer
    {
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is not intended to be used directly", false)]
        public static void Deserialize<T>(ref T obj, int segment, int origin, CapnProtoReader reader, ulong pointer) where T : IBlittable
        {
            // doing this via a generic method allows structs to efficiently use explicitly implemented interfaces;
            // without generics, casting to the interface forces boxing; with generics, it is a "constrained" call -
            // much preferred
            obj.Deserialize(segment, origin, reader, pointer);
        }
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
