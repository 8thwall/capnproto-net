
using System;
using System.IO;
namespace CapnProto
{
    public abstract class SegmentFactory : ISegmentFactory, IRecyclable
    {
        public const int DefaultSegmentWords = 1024;
        private long wordOffset;
        private int defaultSegmentWords;
        protected abstract bool TryReadWord(long wordOffset, out ulong value);

        protected void Init(int defaultSegmentWords)
        {
            this.defaultSegmentWords = defaultSegmentWords;
        }
        void IRecyclable.Reset(bool recyclying)
        {
            Reset(recyclying);
        }
        protected virtual void Reset(bool recyclying)
        {
            defaultSegmentWords = 0;
            wordOffset = 0;
        }

        bool ISegmentFactory.ReadNext(Message message) { return ReadNext(message); }
        protected virtual bool ReadNext(Message message)
        {
            ulong word;
            if (!TryReadWord(wordOffset++, out word)) return false;
            int segments = (int)(uint)(word) + 1;
            // layout is:
            // [count-1][len0]
            // [len1][len2]
            // [len3][len4]
            // ...
            // [lenN][padding]
            // so: we can use count/2 as a sneaky way of knowing how many extra words to expect
            long segmentWordOffset = wordOffset + (segments / 2);
            message.ResetSegments(segments);

            int totalWords = 0;
            for (int i = 0; i < segments; i++)
            {
                if ((i % 2) == 0)
                {
                    word >>= 32;
                }
                else if (!TryReadWord(wordOffset++, out word))
                {
                    throw new EndOfStreamException();
                }
                var segment = message.ReuseExistingSegment() ?? CreateEmptySegment();
                int len = (int)(uint)(word);
                if (!(segment != null && InitializeSegment(segment, segmentWordOffset, len, len)))
                {
                    throw new OutOfMemoryException("Unable to initialize segment " + i);
                }
                message.AddSegment(segment);
                segmentWordOffset += len;
                totalWords += len;
            }
            // move the base offset past the data
            wordOffset += totalWords;
            if (segmentWordOffset != wordOffset)
            {
                throw new InvalidOperationException(string.Format("offset mismatch; craziness; {0} vs {1}", wordOffset, segmentWordOffset));
            }
            return true;
        }
        protected abstract ISegment CreateEmptySegment();

        protected abstract bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords);

        
        /// <summary>
        /// Indicate an amount of available space; the number returned can be anything, but if it is not at least "required",
        /// it will be treated as an allocation failure; the "suggested" value may indicate default block sizes, etc - but
        /// the implementation is welcome to suggest a larger size. Note that the allocation is not considered complete until
        /// InitializeSegment has been successfully invoked (returning true)
        /// </summary>
        protected virtual int SuggestAvailable(Message message, long wordOffset, int requiredWords, int suggestedWords)
        {
            return -1;
        }

        ISegment ISegmentFactory.TryAllocate(Message message, int words)
        {
            if (words <= 0) throw new ArgumentOutOfRangeException("words");
            int suggested = SuggestAvailable(message, wordOffset, words, Math.Max(words, defaultSegmentWords));
            if (suggested < words) return null;

            // k, we're doing ok!
            ISegment seg = CreateEmptySegment();
            if (seg != null && InitializeSegment(seg, wordOffset, suggested, 0))
            {
                message.AddSegment(seg);
                wordOffset += suggested;
                return seg;
            }
            if (seg != null) seg.Dispose();
            return null;
        }

        public virtual void Dispose() { Reset(false); }
    }
}
