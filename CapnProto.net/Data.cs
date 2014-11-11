using System;

namespace CapnProto
{
    public struct Data
    {
        private readonly Pointer pointer;
        private Data(Pointer pointer) {
            
            this.pointer = pointer;
            pointer.AssertNilOrSingleByte();
        }
        public static explicit operator Data(Pointer pointer) { return new Data(pointer); }
        public static implicit operator Pointer(Data obj) { return obj.pointer; }
        public static bool operator true(Data obj) { return obj.pointer.IsValid; }
        public static bool operator false(Data obj) { return !obj.pointer.IsValid; }
        public static Data Create(Pointer parent, int length) { return (Data)parent.AllocateList(ElementSize.OneByte, length); }

        public int Count() { return pointer.SingleByteLength; }
        public void CopyTo(byte[] buffer, int destinationIndex = 0, int sourceIndex = 0, int count = -1) { throw new NotImplementedException(); }

        public int GetByteCount() { return pointer.SingleByteLength; }
        public byte this[int index]
        {
            get { return unchecked((byte)pointer.GetDataWord(index)); }
            set { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFF); }
        }

        public override string ToString()
        {
            return pointer.ToString();
        }
    }
}
