using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapnProto.Take2
{
    public struct AnyPointer
    {
        private readonly Pointer pointer;
        private AnyPointer(Pointer pointer) { this.pointer = pointer; }
        public static implicit operator AnyPointer(Pointer pointer) { return new AnyPointer(pointer); }
        public static explicit operator Pointer(AnyPointer pointer) { return pointer.pointer; }

        public int Count() { throw new NotImplementedException(); }
        public void CopyTo(byte[] buffer, int destinationIndex = 0, int sourceIndex = 0, int count = -1) { throw new NotImplementedException(); }
    }
}
