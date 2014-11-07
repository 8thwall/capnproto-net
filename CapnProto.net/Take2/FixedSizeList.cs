
using System;
namespace CapnProto.Take2
{
    public struct FixedSizeList<T>
    {
        private readonly Pointer pointer;
        private FixedSizeList(Pointer pointer) { this.pointer = pointer; }
        public static implicit operator FixedSizeList<T>(Pointer pointer) { return new FixedSizeList<T>(pointer); }
        public static implicit operator FixedSizeList<T>(AnyPointer pointer) { return new FixedSizeList<T>((Pointer)pointer); }

        public T this[int index] { get { throw new NotImplementedException(); } }
        public int Count() { throw new NotImplementedException(); }
    }
}
