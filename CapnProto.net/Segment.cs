
using System;
using System.Globalization;
using System.Text;
namespace CapnProto
{
    public abstract class Segment : ISegment
    {
        public override string ToString()
        {
            return "Segment " + Index.ToString(CultureInfo.InvariantCulture);
        }
        protected static readonly Encoding Encoding = new UTF8Encoding(false);

        [ThreadStatic]
        private static Decoder sharedDecoder;
        protected static void PushDecoder(Decoder decoder)
        {
            if (decoder != null)
            {
                decoder.Reset();
                sharedDecoder = decoder;
            }
        }
        protected static Decoder PopDecoder()
        {
            var decoder = sharedDecoder;
            sharedDecoder = null;
            if (decoder == null) return Encoding.GetDecoder();
            return decoder;
        }

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

        public virtual void Dispose() { }

        public abstract ulong this[int index] { get; set; }
        public abstract int Length { get; }
        public void Init(Message message, int index)
        {
            this.message = message;
            this.index = index;
        }
        bool ISegment.TryAllocate(int words, out int index)
        {
            return TryAllocate(words, out index);
        }
        protected virtual bool TryAllocate(int words, out int index)
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

#if UNSAFE
        public virtual unsafe int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int space = Length - wordOffset;
            if (maxWords > space) maxWords = space;
            fixed (byte* ptr = &buffer[bufferOffset])
            {
                ulong* typed = (ulong*)ptr;
                for (int i = 0; i < maxWords; i++)
                {
                    typed[i] = this[wordOffset++];
                }
            }
            return maxWords;
        }
        public virtual unsafe int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int space = Length - wordOffset;
            if (maxWords > space) maxWords = space;
            fixed(byte* ptr = &buffer[bufferOffset])
            {
                ulong* typed = (ulong*)ptr;
                for(int i = 0 ; i < maxWords ; i++)
                {
                    this[wordOffset++] = typed[i];
                }
            }
            return maxWords;
        }
#else
        public virtual int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int space = Length - wordOffset;
            if (maxWords > space) maxWords = space;
            
            for (int i = 0; i < maxWords; i++)
            {
                BufferSegment.WriteWord(buffer, bufferOffset, this[wordOffset++]);
                bufferOffset += 8;
            }
            return maxWords;
        }
        public virtual int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int space = Length - wordOffset;
            if (maxWords > space) maxWords = space;
            
            for (int i = 0; i < maxWords; i++)
            {
                this[wordOffset++] = BufferSegment.ReadWord(buffer, bufferOffset);
                bufferOffset += 8;
            }
            return maxWords;
        }
#endif
    }
}
