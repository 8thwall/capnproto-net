//using System;

//namespace CapnProto
//{
//    public struct StructPointer
//    {
//        public StructPointer(ulong value)
//        {
//            if ((value & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Expected struct pointer");
//            this.offset = ((int)value) >> 2;
//            this.dataLength = (ushort)((value >> 32) & 0xFFFF);
//            this.pointerLength = (ushort)((value >> 48) & 0xFFFF);
//        }

//        private int offset;
//        private ushort dataLength;
//        private ushort pointerLength;

//        /// <summary>
//        /// Offset, in words, from the end of the pointer to the start of the struct's data section.
//        /// </summary>
//        public int Offset { get { return offset; } }
//        /// <summary>
//        /// Size of the struct's data section, in words.
//        /// </summary>
//        public uint DataLength { get { return dataLength; } }
//        /// <summary>
//        /// Size of the struct's pointer section, in words.
//        /// </summary>
//        public uint PointerLength { get { return pointerLength; } }
//    }
//}
