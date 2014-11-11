using System;

namespace CapnProto
{
    public struct ListPointer
    {
        public ListPointer(ulong value)
        {
            if ((value & PointerType.Mask) != PointerType.List) throw new InvalidOperationException("Expected list pointer; got " + PointerType.GetName(value));
            this.value = value;
        }

        private readonly ulong value;
        /// <summary>
        /// Offset, in words, from the end of the pointer to the start of the first element of the list. 
        /// </summary>
        public int Offset { get { return unchecked(((int)value) >> 2); } }

        /// <summary>
        /// Size of each element;
        /// 0 = 0 (e.g. List(Void))
        /// 1 = 1 bit
        /// 2 = 1 byte
        /// 3 = 2 bytes
        /// 4 = 4 bytes
        /// 5 = 8 bytes (non-pointer)
        /// 6 = 8 bytes (pointer)
        /// 7 = composite
        /// </summary>
        public ElementSize ElementSize { get { return unchecked((ElementSize)(int)((value >> 32) & 7)); } }

        public int Size { get { return unchecked((int)(value >> 35)); } }

        public static int Parse(ulong pointer, ref int origin, out ElementSize size)
        {
            //lsb                       list pointer                        msb
            //+-+-----------------------------+--+----------------------------+
            //|A|             B               |C |             D              |
            //+-+-----------------------------+--+----------------------------+

            uint first = unchecked((uint)pointer), second = unchecked((uint)(pointer >> 32));

            // A (2 bits) = 1, to indicate that this is a list pointer.
            if ((first & PointerType.Mask) != PointerType.List)
            {
                throw new InvalidOperationException("List header expected; got " + PointerType.GetName(first));
            }
            // B (30 bits) = Offset, in words, from the end of the pointer to the start of the first element of the list.  Signed.
            origin += (int)(first >> 2);
            // C (3 bits) = Size of each element:
            size = (ElementSize)(int)(second & 7);
            // D (29 bits) = Number of elements in the list, except when C is 7
            return (int)(second >> 3);
        }
    }

    public enum ElementSize
    {
        ZeroByte = 0,
        OneBit = 1,
        OneByte = 2,
        TwoBytes = 3,
        FourBytes = 4,
        EightBytesNonPointer = 5,
        EightBytesPointer = 6,
        InlineComposite = 7
    }
}
