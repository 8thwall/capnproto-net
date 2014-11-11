
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
        public static explicit operator string(Text obj) { return obj.ToString(); }

        public static Text Create(Pointer pointer, string value)
        {
            if (value == null) return default(Text);

            if(value.Length == 0)
            {
                // automatic nil pointer
                return (Text)pointer.AllocateList(ElementSize.OneByte, 1);
            }
            int byteLen = Encoding.UTF8.GetByteCount(value);
            var ptr = pointer.AllocateList(ElementSize.OneByte, byteLen + 1);
            ptr.WriteString(value);
            return (Text)ptr;
        }
        public static Text Create(Pointer pointer, char[] value, int offset = 0, int count = -1)
        {
            if (value == null) return default(Text);
            if(count < 0) count = value.Length - offset;
            if (offset + count > value.Length) throw new ArgumentOutOfRangeException("count");

            if(value.Length == 0)
            {
                // automatic nil pointer
                return (Text)pointer.AllocateList(ElementSize.OneByte, 1);
            }

            int byteLen = Encoding.UTF8.GetByteCount(value, offset, count);
            var ptr = pointer.AllocateList(ElementSize.OneByte, byteLen + 1);
            Textizer.Write(ptr, value, offset, count);
            return (Text)ptr;
            
        }
        public override string ToString()
        {
            return pointer.ReadString();
        }
        public int GetByteCount() { return pointer.SingleByteLength; }
        public int GetCharCount() { return Textizer.Count(pointer); }

        public void CopyTo(char[] destination, int destinationIndex) { throw new NotImplementedException(); }

        
        public int AppendTo(StringBuilder destination)
        {
            return Textizer.AppendTo(pointer, destination);
        }
        public int AppendTo(TextWriter destination)
        {
            return Textizer.AppendTo(pointer, destination);
        }
    }
}
