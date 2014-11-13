
using System;
using System.IO;
using System.Text;
namespace CapnProto
{
    public sealed class BufferSegmentFactory : SegmentFactory
    {
        public override void Dispose()
        {
            Cache<BufferSegmentFactory>.Push(this);
        }

        protected override void Reset(bool recycling)
        {
            this.buffer = null;
            this.underlyingBaseOffset = this.availableWords = 0;
            base.Reset(recycling);
        }

        public static BufferSegmentFactory Create(byte[] buffer, int offset = 0, int count = -1, int defaultSegmentWords = DefaultSegmentWords)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (defaultSegmentWords <= 0) throw new ArgumentOutOfRangeException("segmentWords");
            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) { count = buffer.Length - offset; }
            else if (offset + count > buffer.Length) throw new ArgumentOutOfRangeException("count");

            var state = Cache<BufferSegmentFactory>.Pop() ?? new BufferSegmentFactory();
            state.Init(buffer, offset, count, defaultSegmentWords);
            return state;
        }
        private BufferSegmentFactory() { }

        private int underlyingBaseOffset, availableWords;
        private byte[] buffer;

        private void Init(byte[] buffer, int underlyingBaseOffset, int bytes, int defaultSegmentWords)
        {
            this.buffer = buffer;
            this.underlyingBaseOffset = underlyingBaseOffset;
            this.availableWords = bytes >> 3;
            base.Init(defaultSegmentWords);
        }

        protected override int SuggestAvailable(Message message, long wordOffset, int requiredWords, int suggestedWords)
        {
            int space = checked(availableWords - (int)wordOffset);
            return Math.Min(space, suggestedWords);
        }
        protected override unsafe bool TryReadWord(long wordOffset, out ulong value)
        {
            if(wordOffset < availableWords)
            {
                fixed (byte* ptr = &buffer[underlyingBaseOffset])
                {
                    value = ((ulong*)ptr)[wordOffset];
                    return true;
                }
            }
            value = 0;
            return false;
        }
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            ((BufferSegment)segment).Init(buffer, checked(underlyingBaseOffset + (int)(wordOffset << 3)), totalWords, activeWords);
            return true;
        }
        protected override ISegment CreateEmptySegment()
        {
            return BufferSegment.Create();
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
