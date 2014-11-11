//using System;
//using System.ComponentModel;
//using System.IO;
//using System.Runtime.CompilerServices;

//namespace CapnProto
//{
//    public static class TypeSerializer
//    {
//        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
//        [Obsolete("This method is not intended to be used directly", false)]
//        [MethodImpl(MethodImplOptions.AggressiveInlining)]
//        public static void Deserialize<T>(ref T obj, int segment, int origin, DeserializationContext ctx, ulong pointer) where T : IBlittable
//        {
//            // doing this via a generic method allows structs to efficiently use explicitly implemented interfaces;
//            // without generics, casting to the interface forces boxing; with generics, it is a "constrained" call -
//            // much preferred
//            TypeModel.Log("{0}: Deserializing {1}", ctx.Depth, typeof(T).Name);
//            ctx.StepIn();
//            if((pointer & PointerType.Mask) == PointerType.Far)
//            {
//                pointer = ctx.Reader.ResolveFarPointer(pointer, ref segment, ref origin);
//            }
//            obj.Deserialize(segment, origin, ctx, pointer);
//            ctx.StepOut();
//        }
//        public static object Deserialize(this ITypeSerializer serializer, Stream source)
//        {
//            if (serializer == null) throw new ArgumentNullException("serializer");
//            if (source == null) throw new ArgumentNullException("source");
//            using(var reader = CapnProtoReader.Create(source, null))
//            {
//                return Deserialize(serializer, reader);
//            }
//        }
//        public static T Deserialize<T>(this ITypeSerializer<T> serializer, Stream source)
//        {
//            if (serializer == null) throw new ArgumentNullException("serializer");
//            if (source == null) throw new ArgumentNullException("source");
//            using (var reader = CapnProtoReader.Create(source, null))
//            {
//                return Deserialize<T>(serializer, reader);
//            }
//        }
//        public static object Deserialize(this ITypeSerializer serializer, CapnProtoReader source)
//        {
//            if (serializer == null) throw new ArgumentNullException("serializer");
//            if (source == null) throw new ArgumentNullException("source");

//            if(source.SegmentCount == 0) source.ReadSegmentsHeader();
//            var ctx = new DeserializationContext(serializer.Model, source, source.Context);

//            // The first word of the first segment of the message is always a pointer pointing to the message’s root struct.
//            var ptr = ctx.Reader.ReadWord(0, 0);
//            return serializer.Deserialize(0, 1, ctx, ptr);
//        }
//        public static T Deserialize<T>(this ITypeSerializer<T> serializer, CapnProtoReader source)
//        {
//            if (serializer == null) throw new ArgumentNullException("serializer");
//            if (source == null) throw new ArgumentNullException("source");

//            if (source.SegmentCount == 0) source.ReadSegmentsHeader();
//            var ctx = new DeserializationContext(serializer.Model, source, source.Context);

//            // The first word of the first segment of the message is always a pointer pointing to the message’s root struct.
//            var ptr = ctx.Reader.ReadWord(0, 0);
//            return serializer.Deserialize(0, 1, ctx, ptr);
//        }
//    }
//}
