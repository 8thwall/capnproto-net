
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
namespace CapnProto
{
    public struct Text : IPointer
    {
        public static explicit operator Text(Pointer pointer) { return new Text(pointer, null); }
        public static implicit operator Pointer(Text pointer) { return pointer.pointer; }
        public static explicit operator string(Text obj) { return obj.ToString(); }

        private static readonly TextComparer comparer = new TextComparer();
        public static TextComparer Comparer { get { return comparer; } }
        public sealed class TextComparer : IComparer<Text>, IEqualityComparer<Text>
        {
            public int Compare(Text x, Text y)
            {
                Pointer xPtr = x.pointer, yPtr = y.pointer;
                if (xPtr == yPtr) return 0;

                if (xPtr.IsValid)
                {
                    if (yPtr.IsValid)
                    {
                        xPtr = xPtr.Dereference();
                        yPtr = yPtr.Dereference();
                        if (xPtr == yPtr) return 0;

                        // TODO: replace with byte work
                        return string.Compare(xPtr.ReadString(), yPtr.ReadString());
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (yPtr.IsValid)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }

            public bool Equals(Text x, Text y)
            {
                Pointer xPtr = x.pointer, yPtr = y.pointer;
                if (xPtr == yPtr) return true;
                
                if(xPtr.IsValid)
                {
                    if(yPtr.IsValid)
                    {
                        xPtr = xPtr.Dereference();
                        yPtr = yPtr.Dereference();
                        if (xPtr == yPtr) return true;

                        // TODO: replace with byte work
                        return xPtr.ReadString() == yPtr.ReadString();
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (yPtr.IsValid)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            public int GetHashCode(Text obj)
            {
                var ptr = obj.pointer;
                if (!ptr.IsValid) return 0;

                ptr = ptr.Dereference();

                // TODO: replace with byte work
                return ptr.ReadString().GetHashCode();
            }
        }

        private Pointer pointer;
        private string value;
        private Text(Pointer pointer, string value)
        {
            this.pointer = pointer;
            this.value = value;
        }

        public static Text Create(Pointer pointer, string value)
        {
            if (value == null) return default(Text);

            if (value.Length == 0)
            {
                // automatic nil pointer
                return (Text)pointer.AllocateList(ElementSize.OneByte, 1);
            }
            int byteLen = Encoding.UTF8.GetByteCount(value);
            var ptr = pointer.AllocateList(ElementSize.OneByte, byteLen + 1);
            ptr.WriteString(value);
            return new Text(ptr, value);
        }
        public static Text Create(Pointer pointer, char[] value, int offset = 0, int count = -1)
        {
            if (value == null) return default(Text);
            if (count < 0) count = value.Length - offset;
            if (offset + count > value.Length) throw new ArgumentOutOfRangeException("count");

            if (value.Length == 0)
            {
                // automatic nil pointer
                return (Text)pointer.AllocateList(ElementSize.OneByte, 1);
            }

            int byteLen = Encoding.UTF8.GetByteCount(value, offset, count);
            var ptr = pointer.AllocateList(ElementSize.OneByte, byteLen + 1);
            Textizer.Write(ptr, value, offset, count);
            return (Text)ptr;

        }
        public static Text Create(Pointer pointer, IDataRecord reader, int fieldIndex)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            if (fieldIndex < 0 || fieldIndex >= reader.FieldCount) throw new ArgumentOutOfRangeException("fieldIndex");
            return Textizer.Create(pointer, reader, fieldIndex);
        }
        public override string ToString()
        {
            return value ?? (value = pointer.ReadString());
        }
        public int GetByteCount() { return pointer.SingleByteLength; }
        public int GetCharCount()
        {
            return value == null ? Textizer.Count(pointer) : value.Length;
        }

        public void CopyTo(char[] destination, int destinationIndex)
        {
            throw new NotImplementedException();
        }


        public int AppendTo(StringBuilder destination)
        {
            if (value != null)
            {
                destination.Append(value);
                return value.Length;
            }
            return Textizer.AppendTo(pointer, destination);
        }
        public int AppendTo(TextWriter destination)
        {
            if(value != null)
            {
                destination.Write(value);
                return value.Length;
            }
            return Textizer.AppendTo(pointer, destination);
        }
        public override bool Equals(object obj)
        {
            return obj is Text && ((Text)obj).pointer == this.pointer;
        }
        public override int GetHashCode() { return this.pointer.GetHashCode(); }
        Pointer IPointer.Pointer { get { return pointer; } }
    }
}
