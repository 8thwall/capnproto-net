
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
            public int Offset {get;private set;}
            public int Count {get;private set;}
            public byte[] Buffer {get;private set;}

            public void Consume(int count)
            {
                if (count < 0) throw new ArgumentOutOfRangeException("count");
                if (count > Count) throw new EndOfStreamException();
                Offset += count;
                Count -= count;
            }
            public BufferState() {}

            public void Init(byte[] buffer, int offset, int count)
            {
                Buffer = buffer;
                Offset = offset;
                Count = count;
            }

            public int ReadInt32()
            {
                if (Count < 4) throw new EndOfStreamException();
                var result = BitConverter.ToInt32(Buffer, Offset);
                Offset += 4;
                Count -= 4;
                return result;
            }
        }
        public static ISegmentFactoryState Initialize(byte[] buffer, int offset = 0, int count = -1)
        {
            if (buffer == null) throw new ArgumentNullException();
            if (offset < 0 || offset >= buffer.Length) throw new ArgumentOutOfRangeException("offset");
            if (count < 0) { count = buffer.Length - offset; }
            else if (offset + count >= buffer.Length) throw new ArgumentOutOfRangeException("count");

            var state = new BufferState();
            state.Init(buffer, offset, count);
            return state;
        }

        public bool ReadNext(object state, Message message)
        {
            if(state==null) throw new ArgumentNullException("state");
            if(message==null) throw new ArgumentNullException("state");

            var ctx = (BufferState)state;
            if(ctx.Count == 0) return false;

            int segments = ctx.ReadInt32() + 1;
            int len0 = ctx.ReadInt32();
            // layout is:
            // [count-1][len0]
            // [len1][len2]
            // [len3][len4]
            // ...
            // [lenN][padding]
            // so: we can use count/2 as a sneaky way of knowing how many extra words to expect

            int origin = ctx.Offset + 8 * (segments / 2);
            message.ResetSegments(segments);
            var buffer = ctx.Buffer;
            int totalWords = 0;
            for (int i = 0; i < segments; i++)
            {
                int len = i == 0 ? len0 : ctx.ReadInt32();

                var segment = (BufferSegment)message.ReuseExistingSegment() ?? BufferSegment.Create();
                segment.Init(buffer, origin, len, len);
                message.AddSegment(segment);
                origin += len * 8;
                totalWords += len;
            }
            // move the base offset past the data
            // note: if we have an even number of segments, then we need to move past the padding too
            ctx.Consume((8 * totalWords) + ((segments % 2) == 0 ? 4 : 0));
            return true;
        }

        private class BufferSegment : ISegment
        {
            
            private byte[] buffer;
            private Message message;
            private int offset, wordCount, index, spaceWords, writeIndex;
            
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
            void ISegment.Init(Message message, int index)
            {
                this.message = message;
                this.index = index;
            }

            public int Index { get { return index; } }

            public Message Message { get { return message; } }

            public ulong this[int index]
            {
                get { return BitConverter.ToUInt64(buffer, offset + (index * 8)); }
            }

            void IRecyclable.Reset(bool recycling)
            {
                buffer = null;
                message = null;
            }


            bool ISegment.TryAllocate(int size, out uint index)
            {
                int space = wordCount - writeIndex;
                if(space >= size)
                {
                    index = (uint)writeIndex;
                    writeIndex += size;
                    return true;
                }
                index = uint.MaxValue;
                return false;
            }
        }
    }
}
