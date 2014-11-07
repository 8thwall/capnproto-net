using System;

namespace CapnProto.Take2
{
    public struct Data
    {
        private readonly Pointer pointer;
        private Data(Pointer pointer) { this.pointer = pointer; }
        public static implicit operator Data(Pointer pointer) { return new Data(pointer); }
        public static implicit operator Data(AnyPointer pointer) { return new Data((Pointer)pointer); }

        public int Count() { throw new NotImplementedException(); }
        public void CopyTo(byte[] buffer, int destinationIndex = 0, int sourceIndex = 0, int count = -1) { throw new NotImplementedException(); }
    }
}
