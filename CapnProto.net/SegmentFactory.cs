
using System;
using System.IO;
namespace CapnProto
{
    abstract class SegmentFactory : ISegmentFactory
    {
        private long wordOffset;

        protected abstract bool TryReadUInt64(long offset, out ulong value);

        bool ISegmentFactory.ReadNext(Message message)
        {
            ulong word;
            if (!TryReadUInt64(wordOffset++, out word)) return false;
            int segments = (int)(uint)(word) + 1;
            // layout is:
            // [count-1][len0]
            // [len1][len2]
            // [len3][len4]
            // ...
            // [lenN][padding]
            // so: we can use count/2 as a sneaky way of knowing how many extra words to expect
            long origin = wordOffset + (segments / 2);
            message.ResetSegments(segments);

            int totalWords = 0;
            for (int i = 0; i < segments; i++)
            {
                if ((i % 2) == 0)
                {
                    word >>= 32;
                }
                else if (!TryReadUInt64(wordOffset++, out word))
                {
                    throw new EndOfStreamException();
                }
                var segment = message.ReuseExistingSegment() ?? CreateEmptySegment();
                int len = (int)(uint)(word);
                InitializeSegment(segment, origin, len, len);
                message.AddSegment(segment);
                origin += (len << 3);
                totalWords += len;
            }
            // move the base offset past the data
            wordOffset += totalWords;
            if (origin != wordOffset)
            {
                throw new InvalidOperationException(string.Format("offset mismatch; craziness; {0} vs {1}", wordOffset, origin));
            }
            return true;
        }
        protected abstract ISegment CreateEmptySegment();

        protected abstract void InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords);

        public virtual ISegment TryAllocate(Message message, int words)
        {
            return null;
        }

        public virtual void Dispose() { }
    }
}
