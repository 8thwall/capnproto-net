using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    public struct ListPointer
    {
        public ListPointer(ulong value)
        {
            if ((value & 3) != 1) throw new InvalidOperationException("Expected list pointer");
            this.offset = ((int)value) >> 2;
            this.combinedSize = (int)(value >> 32);
        }
        private readonly int offset, combinedSize;
        /// <summary>
        /// Offset, in words, from the end of the pointer to the start of the first element of the list. 
        /// </summary>
        public int Offset { get { return offset; } }

        /// <summary>
        /// Size of each element;
        /// 0 = 0 (e.g. List(Void))
        /// 1 = 1 bit
        /// 2 = 1 byte
        /// 3 = 2 bytes
        /// 4 = 4 bytes
        /// 5 = 8 bytes (non-pointer)
        /// 6 = 8 bytes (pointer)
        /// 7 = composite (see below)
        /// </summary>
        public ElementSize ElementSize { get { return (ElementSize)(combinedSize & 7); } }

        public int Size { get { return combinedSize >> 3; } }
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
        Composite = 7
    }
}
