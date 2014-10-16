using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    public struct StructPointer
    {
        internal StructPointer(int offset, ushort dataLength, ushort pointerLength)
        {
            this.offset = offset;
            this.dataLength = dataLength;
            this.pointerLength = pointerLength;
        }
        private int offset;
        private ushort dataLength;
        private ushort pointerLength;

        /// <summary>
        /// Offset, in words, from the end of the pointer to the start of the struct's data section.
        /// </summary>
        public int Offset { get { return offset; } }
        /// <summary>
        /// Size of the struct's data section, in words.
        /// </summary>
        public int DataLength { get { return dataLength; } }
        /// <summary>
        /// Size of the struct's pointer section, in words.
        /// </summary>
        public int PointerLength { get { return pointerLength; } }
    }
}
