
using System;
namespace CapnProto.Take2
{
    public struct FixedSizeList<T>
    {
        private readonly Pointer pointer;
        private FixedSizeList(Pointer pointer) { this.pointer = pointer; }
        public static explicit operator FixedSizeList<T>(Pointer pointer) { return new FixedSizeList<T>(pointer); }
        public static implicit operator Pointer(FixedSizeList<T> obj) { return obj.pointer; }
        public static bool operator true(FixedSizeList<T> obj) { return obj.pointer.IsValid; }
        public static bool operator false(FixedSizeList<T> obj) { return !obj.pointer.IsValid; }

        public T this[int index] { get { throw new NotImplementedException(); } }
        public int Count() { throw new NotImplementedException(); }
    }
}
