
using System;
using System.IO;
using System.Text;
namespace CapnProto.Take2
{
    public struct Text
    {
        private readonly Pointer pointer;
        private Text(Pointer pointer){ this.pointer = pointer; }
        public static implicit operator Text(Pointer pointer) { return new Text(pointer); }
        public static implicit operator Text(AnyPointer pointer) { return new Text((Pointer)pointer); }
        public override string ToString()
        {
            throw new NotImplementedException();
        }
        public int GetByteCount() { throw new NotImplementedException(); }
        public int GetCharCount() { throw new NotImplementedException(); }

        public void CopyTo(char[] destination, int destinationIndex) { throw new NotImplementedException(); }
        public void AppendTo(StringBuilder destination) { throw new NotImplementedException(); }
        public void AppendTo(TextWriter destination) { throw new NotImplementedException(); }
    }
}
