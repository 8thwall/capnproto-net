
using System;
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
#if UNSAFE
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
#else
        protected override bool TryReadWord(long wordOffset, out ulong value)
        {
            if(wordOffset < availableWords)
            {
                value = BufferSegment.ReadWord(buffer, underlyingBaseOffset + (int)(wordOffset << 3));
                return true;
            }
            value = 0;
            return false;
        }
        
#endif
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            ((BufferSegment)segment).Init(buffer, checked(underlyingBaseOffset + (int)(wordOffset << 3)), totalWords, activeWords);
            return true;
        }
        protected override ISegment CreateEmptySegment()
        {
            return BufferSegment.Create();
        }
    }
}
