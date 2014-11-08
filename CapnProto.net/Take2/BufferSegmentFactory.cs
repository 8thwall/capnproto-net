
using System;
using System.IO;
namespace CapnProto.Take2
{
    public sealed class BufferSegmentFactory : ISegmentFactory
    {
        private BufferSegmentFactory() {}
        private static readonly BufferSegmentFactory instance = new BufferSegmentFactory();
        public static BufferSegmentFactory Instance { get { return instance; }}
        private class BufferState : ISegmentFactoryState
        {
            private int segmentSize;
            private int offset;
            private int count;
            private byte[] buffer;

            public void Consume(int count)
            {
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                if (count > this.count) throw new EndOfStreamException();
                this.offset += count;
                this.count -= count;
            }
            public BufferState() {}

            public void Init(byte[] buffer, int offset, int count, int segmentSize)
            {
                this.segmentSize = segmentSize;
                this.buffer = buffer;
                this.offset = offset;
                this.count = count;
            }

            public int ReadInt32()
            {
                if (this.count < 4) throw new EndOfStreamException();
                var result = BitConverter.ToInt32(buffer, offset);
                offset += 4;
                count -= 4;
                return result;
            }
            internal BufferSegment TryAllocate(Message message, int size)
            {
                BufferSegment segment = null;
                if (count >= size)
                {
                    // definitely have enough
                    if (segmentSize > size)
                    {
                        size = segmentSize; // allocate an entire segment
                        if (size > count) size = count; // unless that makes us too big
                    }
                    segment = (BufferSegment)message.ReuseExistingSegment() ?? BufferSegment.Create();
                    segment.Init(buffer, offset, size, 0);
                    Consume(size);
                    message.AddSegment(segment);
                }
                return segment;
            }
            internal bool ReadNext(Message message)
            {
                if (count == 0) return false;

                int segments = ReadInt32() + 1;
                int len0 = ReadInt32();
                // layout is:
                // [count-1][len0]
                // [len1][len2]
                // [len3][len4]
                // ...
                // [lenN][padding]
                // so: we can use count/2 as a sneaky way of knowing how many extra words to expect

                int origin = offset + 8 * (segments / 2);
                message.ResetSegments(segments);
                var buffer = this.buffer;
                int totalWords = 0;
                for (int i = 0; i < segments; i++)
                {
                    int len = i == 0 ? len0 : ReadInt32();

                    var segment = (BufferSegment)message.ReuseExistingSegment() ?? BufferSegment.Create();
                    segment.Init(buffer, origin, len, len);
                    message.AddSegment(segment);
                    origin += len * 8;
                    totalWords += len;
                }
                // move the base offset past the data
                // note: if we have an even number of segments, then we need to move past the padding too
                Consume((8 * totalWords) + ((segments % 2) == 0 ? 4 : 0));
                return true;
            }
        }

        ISegment ISegmentFactory.TryAllocate(Message message, int size)
        {
            return ((BufferState)message.FactoryState).TryAllocate(message, size);
        }
        bool ISegmentFactory.ReadNext(Message message)
        {
            return ((BufferState)message.FactoryState).ReadNext(message);
        }

        public static ISegmentFactoryState Initialize(byte[] buffer, int offset = 0, int count = -1, int segmentSize = 1024)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (segmentSize <= 0) throw new ArgumentOutOfRangeException("segmentSize");
            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) { count = buffer.Length - offset; }
            else if (offset + count >= buffer.Length) throw new ArgumentOutOfRangeException("count");

            var state = new BufferState();
            state.Init(buffer, offset, count, segmentSize);
            return state;
        }

        private class BufferSegment : Segment
        {
            
            private byte[] buffer;
            private int offset, wordCount, writeIndex;
            
            public static BufferSegment Create()
            {
                return new BufferSegment();
            }
            private BufferSegment() { }
            public void Init(byte[] buffer, int offset, int wordCount, int writeIndex)
            {
                this.buffer = buffer;
                this.offset = offset;
                this.wordCount = wordCount;
                this.writeIndex = writeIndex;
            }
            public override unsafe ulong this[int index]
            {
                get {
                    fixed (byte* ptr = &buffer[offset])
                    {
                        return ((ulong*)ptr)[index];
                    }
                }
                set
                {
                    fixed(byte* ptr = &buffer[offset])
                    {
                        ((ulong*)ptr)[index] = value;
                    }
                }
            }

            public override void Reset(bool recycling)
            {
                buffer = null;
            }
            public override bool TryAllocate(int size, out int index)
            {
                int space = wordCount - writeIndex;
                if(space >= size)
                {
                    index = writeIndex;
                    writeIndex += size;
                    return true;
                }
                index = int.MinValue;
                return false;
            }
        }
    }
}
