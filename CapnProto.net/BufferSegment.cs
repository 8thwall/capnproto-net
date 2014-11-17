using System;
using System.Text;

namespace CapnProto
{
    public class BufferSegment : Segment
    {

        private byte[] buffer;
        private int offset, capacityWords, activeWords;

        public static BufferSegment Create()
        {
            return new BufferSegment();
        }
        private BufferSegment() { }
        public void Init(byte[] buffer, int offset, int capacityWords, int activeWords)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.capacityWords = capacityWords;
            this.activeWords = activeWords;
            if (activeWords < capacityWords) Array.Clear(buffer, offset, capacityWords - activeWords);
        }
#if UNSAFE
        public override unsafe ulong this[int index]
        {
            get
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    return ((ulong*)ptr)[index];
                }
            }
            set
            {
                fixed (byte* ptr = &buffer[offset])
                {
                    ((ulong*)ptr)[index] = value;
                }
            }
        }

        public override unsafe void SetValue(int index, ulong value, ulong mask)
        {
            fixed (byte* ptr = &buffer[offset])
            {
                ulong* typed = (ulong*)ptr;
                typed[index] = (value & mask) | (typed[index] & ~mask);
            }
        }
#else
        internal static ulong ReadWord(byte[] buffer, int offset)
        {
            unchecked
            {
                int lhs = buffer[offset++] | (buffer[offset++] << 8) | (buffer[offset++] << 16) | (buffer[offset++] << 24);
                int rhs = buffer[offset++] | (buffer[offset++] << 8) | (buffer[offset++] << 16) | (buffer[offset] << 24);
                return ((ulong)(uint)lhs) | (((ulong)(uint)rhs) << 32);
            }
        }
        internal static void WriteWord(byte[] buffer, int offset, ulong value)
        {
            unchecked
            {
                buffer[offset++] = (byte)value;
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 24);
                buffer[offset++] = (byte)(value >> 32);
                buffer[offset++] = (byte)(value >> 40);
                buffer[offset++] = (byte)(value >> 48);
                buffer[offset] = (byte)(value >> 56);
            }
        }
        internal static void WriteNibble(byte[] buffer, int offset, uint value)
        {
            unchecked
            {
                buffer[offset++] = (byte)value;
                buffer[offset++] = (byte)(value >> 8);
                buffer[offset++] = (byte)(value >> 16);
                buffer[offset++] = (byte)(value >> 24);
            }
        }
        public override ulong this[int index]
        {
            get
            {
                return ReadWord(buffer, offset + (index << 3));
            }
            set
            {

                WriteWord(buffer, offset + (index << 3), value);
            }
        }
#endif

        public override void Reset(bool recycling)
        {
            buffer = null;
        }
        protected override bool TryAllocate(int words, out int index)
        {
            int space = capacityWords - activeWords;
            if (space >= words)
            {
                index = activeWords;
                activeWords += words;
#if VERBOSE
                    Console.WriteLine(string.Format(
                        "Allocating {0} words at [{1}:{2}-{3}]", words, Index, index, index + words - 1));
#endif
                return true;
            }
            index = int.MinValue;
            return false;
        }
        public override int Length
        {
            get { return activeWords; }
        }

        public override int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int wordsToCopy = activeWords - wordOffset;
            if (wordsToCopy > maxWords) wordsToCopy = maxWords;

            Buffer.BlockCopy(this.buffer, offset + (wordOffset << 3), buffer, bufferOffset, wordsToCopy << 3);
            return wordsToCopy;
        }
        public override int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int wordsToCopy = activeWords - wordOffset;
            if (wordsToCopy > maxWords) wordsToCopy = maxWords;

            Buffer.BlockCopy(buffer, bufferOffset, this.buffer, offset + (wordOffset << 3), wordsToCopy << 3);
            return wordsToCopy;
        }
        public override int WriteString(int index, string value, int bytes)
        {
            if (bytes-- > 0)
            {
                int offset = this.offset + (index << 3);
                if (this.buffer[offset + bytes] == (byte)0)
                {
                    return Encoding.GetBytes(value, 0, value.Length, this.buffer, offset);
                }
            }
            throw new InvalidOperationException();
        }

        public override string ReadString(int index, int bytes)
        {
            if (bytes-- > 0)
            {
                int offset = this.offset + (index << 3);
                if (buffer[offset + bytes] == (byte)0)
                {
                    return Encoding.GetString(buffer, offset, bytes);
                }
            }
            throw new InvalidOperationException();
        }
    }
}
