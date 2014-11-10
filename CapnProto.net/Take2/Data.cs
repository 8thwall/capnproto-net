using System;

namespace CapnProto.Take2
{
    public struct Data
    {
        private readonly Pointer pointer;
        private Data(Pointer pointer) {
            
            this.pointer = pointer;
            pointer.AssertNilOrSingleByte();
        }
        public static implicit operator Data(Pointer pointer) { return new Data(pointer); }
        public static implicit operator Pointer(Data obj) { return obj.pointer; }
        public static bool operator true(Data obj) { return obj.pointer.IsValid; }
        public static bool operator false(Data obj) { return !obj.pointer.IsValid; }
        public static Data Create(Pointer parent, int length) { return parent.Allocate(ElementSize.OneByte, length); }
        
        public int Count() { throw new NotImplementedException(); }
        public void CopyTo(byte[] buffer, int destinationIndex = 0, int sourceIndex = 0, int count = -1) { throw new NotImplementedException(); }

        public int Length { get { return pointer.SingleByteLength; } }
        public byte this[int index]
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
    }
}
