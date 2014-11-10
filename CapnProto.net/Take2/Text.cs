
using System;
using System.IO;
using System.Text;
namespace CapnProto.Take2
{
    public struct Text
    {
        private readonly Pointer pointer;
        private Text(Pointer pointer){ this.pointer = pointer; }
        public static explicit operator Text(Pointer pointer) { return new Text(pointer); }
        public static implicit operator Pointer(Text pointer) { return pointer.pointer; }
        public static bool operator true(Text obj) { return obj.pointer.IsValid; }
        public static bool operator false(Text obj) { return !obj.pointer.IsValid; }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
        public int GetByteCount() { return pointer.SingleByteLength; }
        public int GetCharCount() { throw new NotImplementedException(); }

        public void CopyTo(char[] destination, int destinationIndex) { throw new NotImplementedException(); }
        public void AppendTo(StringBuilder destination) { throw new NotImplementedException(); }
        public void AppendTo(TextWriter destination) { throw new NotImplementedException(); }

        public int ByteLength { get { return pointer.SingleByteLength; } }
        public int CharLength { get { throw new NotImplementedException(); } }
    }
}
