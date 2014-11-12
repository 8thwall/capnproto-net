
using System;
using System.IO;
using System.Text;
namespace CapnProto
{
    public sealed class BufferSegmentFactory : ISegmentFactory, IRecyclable
    {
        void IRecyclable.Reset(bool recycling)
        {
            this.buffer = null;
            this.offsetBytes = this.remainingWords = this.segmentWords = 0;
        }
        void IDisposable.Dispose()
        {
            Cache<BufferSegmentFactory>.Push(this);
        }
        public static BufferSegmentFactory Create(byte[] buffer, int offset = 0, int count = -1, int segmentWords = 1024)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (segmentWords <= 0) throw new ArgumentOutOfRangeException("segmentWords");
            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) { count = buffer.Length - offset; }
            else if (offset + count > buffer.Length) throw new ArgumentOutOfRangeException("count");

            var state = Cache<BufferSegmentFactory>.Pop() ?? new BufferSegmentFactory();
            state.Init(buffer, offset, count, segmentWords);
            return state;
        }
        private BufferSegmentFactory() { }

        private int segmentWords;
        private int offsetBytes;
        private int remainingWords;
        private byte[] buffer;

        private void Init(byte[] buffer, int offsetBytes, int bytes, int segmentWords)
        {
            this.segmentWords = segmentWords;
            this.buffer = buffer;
            this.offsetBytes = offsetBytes;
            this.remainingWords = bytes >> 3;
        }


        private void ConsumeWords(int words)
        {
            if (words < 0) throw new ArgumentOutOfRangeException("count");
            if (words > this.remainingWords) throw new EndOfStreamException();
            this.offsetBytes += words << 3;
            this.remainingWords -= words;
        }

        private unsafe ulong ReadUInt64()
        {
            if (this.remainingWords < 1) throw new EndOfStreamException();
            ulong result;
            fixed (byte* ptr = &buffer[offsetBytes])
            {
                result = *((ulong*)ptr);
            }
            offsetBytes += 8;
            remainingWords--;
            return result;
        }
        ISegment ISegmentFactory.TryAllocate(Message message, int words)
        {
            BufferSegment segment = null;
            if (remainingWords >= words)
            {
                // definitely have enough
                if (segmentWords > words)
                {
                    words = segmentWords; // allocate an entire segment
                    if (words > remainingWords) words = remainingWords; // unless that makes us too big
                }
                segment = (BufferSegment)message.ReuseExistingSegment() ?? BufferSegment.Create();
                segment.Init(buffer, offsetBytes, words, 0);
                ConsumeWords(words);
                message.AddSegment(segment);
            }
            return segment;
        }
        bool ISegmentFactory.ReadNext(Message message)
        {
            if (remainingWords == 0) return false;

            ulong word = ReadUInt64();
            int segments = (int)(uint)(word) + 1;
            // layout is:
            // [count-1][len0]
            // [len1][len2]
            // [len3][len4]
            // ...
            // [lenN][padding]
            // so: we can use count/2 as a sneaky way of knowing how many extra words to expect
            int origin = offsetBytes + 8 * (segments / 2);
            message.ResetSegments(segments);
            var buffer = this.buffer;

            int totalWords = 0;
            for (int i = 0; i < segments; i++)
            {
                if ((i % 2) == 0)
                {
                    word >>= 32;
                }
                else
                {
                    word = ReadUInt64();
                }
                var segment = (BufferSegment)message.ReuseExistingSegment() ?? BufferSegment.Create();
                int len = (int)(uint)(word);
                segment.Init(buffer, origin, len, len);
                message.AddSegment(segment);
                origin += (len << 3);
                totalWords += len;
            }
            // move the base offset past the data
            ConsumeWords(totalWords);
            if (origin != offsetBytes)
            {
                throw new InvalidOperationException(string.Format("offset mismatch; craziness; {0} vs {1}", offsetBytes, origin));
            }
            return true;
        }

        private class BufferSegment : Segment
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

            public override void Reset(bool recycling)
            {
                buffer = null;
            }
            public override bool TryAllocate(int words, out int index)
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
            public override unsafe int WriteString(int index, string value, int bytes)
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
}
