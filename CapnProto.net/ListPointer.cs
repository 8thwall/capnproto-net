using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    public struct ListPointer
    {
        public ListPointer(int offset, int combinedSize)
        {
            this.combinedSize = combinedSize;
            this.offset = offset;
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
        public int ElementSize { get { return combinedSize & 7; } }

        public int Size { get { return combinedSize << 3; } }
    }
}
