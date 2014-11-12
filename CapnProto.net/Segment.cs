
using System;
using System.Text;
namespace CapnProto
{
    public abstract class Segment : ISegment
    {
        protected static readonly Encoding Encoding = new UTF8Encoding(false);
        public int Index
        {
            get { return index; }
        }
        private Message message;
        private int index;
        public Message Message
        {
            get { return message; }
        }

        public abstract ulong this[int index] { get; set; }
        public abstract int Length { get; }
        public void Init(Message message, int index)
        {
            this.message = message;
            this.index = index;
        }

        public virtual bool TryAllocate(int words, out int index)
        {
            index = int.MinValue;
            return false;
        }

        public virtual void Reset(bool recycling)
        {
            message = null;
        }


        public virtual void SetValue(int index, ulong value, ulong mask)
        {
            this[index] = (value & mask) | (this[index] & ~mask);
        }
        public virtual int WriteString(int index, string value, int bytes)
        {
            throw new NotImplementedException();
        }
        public virtual string ReadString(int index, int bytes)
        {
            throw new NotImplementedException();
        }

        public virtual int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int words)
        {
            throw new NotImplementedException();
        }
        public virtual int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int words)
        {
            int space = Length - wordOffset;
            if (words > space) words = space;
            for(int i = 0 ; i < words ; i++)
            {
                this[wordOffset++] =
                    (ulong)buffer[bufferOffset++] | ((ulong)buffer[bufferOffset++] << 8) |
                    ((ulong)buffer[bufferOffset++] << 16) | ((ulong)buffer[bufferOffset++] << 24) |
                    ((ulong)buffer[bufferOffset++] << 32) | ((ulong)buffer[bufferOffset++] << 40) |
                    ((ulong)buffer[bufferOffset++] << 48) | ((ulong)buffer[bufferOffset++] << 56);
            }
            return words;
        }
    }
}
