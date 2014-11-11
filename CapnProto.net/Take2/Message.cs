
using System;
using System.Collections.Generic;
using System.IO;
namespace CapnProto.Take2
{
    public sealed class Message : IDisposable, IRecyclable, ISegment, IEnumerable<ISegment>
    {
        public const int WordLength = 8;
        private Message() { }

        public static Message Create(ISegmentFactory segmentFactory)
        {
            var msg = Cache<Message>.Pop() ?? new Message();
            msg.Init(segmentFactory);
            return msg;
        }

        public static implicit operator Pointer(Message message)
        {
            return message == null ? default(Pointer) : message.Root;
        }

        int ISegment.Index { get { return 0; } }
        Message ISegment.Message { get { return this; } }
        ulong ISegment.this[int index]
        {
            get
            {
                if (index == 0 && SegmentCount == 0) return 0;
                return this[0][index];
            }
            set { this[0][index] = value; }
        }
        void ISegment.SetValue(int index, ulong value, ulong mask)
        {
            this[0].SetValue(index, value, mask);
        }
        void ISegment.Init(Message message, int index) { }
        bool ISegment.TryAllocate(int size, out int index)
        {
            if (SegmentCount == 0)
            {
                ISegment seg = CreateSegment(size + 1); // need 1 for the root header
                if (seg.TryAllocate(size + 1, out index))
                {
                    index++; // leave a space for the header
                    return true;
                }
                throw new OutOfMemoryException(string.Format("Unable to allocate {0} words for the root object", size + 1));
            }
            return this[0].TryAllocate(size, out index);
        }
        int ISegment.Length { get { return SegmentCount == 0 ? 0 : this[0].Length; } }

        int ISegment.WriteString(int index, string value, int bytes) { return this[0].WriteString(index, value, bytes); }
        string ISegment.ReadString(int index, int bytes) { return this[0].ReadString(index, bytes); }
        int ISegment.ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords) { return this[0].ReadWords(wordOffset, buffer, bufferOffset, maxWords); }
        int ISegment.WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords) { return this[0].WriteWords(wordOffset, buffer, bufferOffset, maxWords); }
        public override string ToString()
        {
            return string.Format("{0} segments, {1} words", SegmentCount, WordCount);
        }
        public long WordCount
        {
            get
            {
                long totalWords = 0;
                int count = SegmentCount;
                for (int i = 0; i < count; i++)
                {
                    totalWords += this[i].Length;
                }
                return totalWords;
            }
        }
        public string ToString(long word)
        {
            if (word >= 0)
            {
                int count = SegmentCount;
                for (int i = 0; i < count; i++)
                {
                    var seg = this[i];
                    int segLen = seg.Length;
                    if (word < segLen) return seg.ToString((int)word);
                    word -= segLen;
                }
            }
            throw new IndexOutOfRangeException("word");
        }
        

        public Pointer Root
        {
            get
            {
                if (SegmentCount == 0)
                {
                    return new Pointer(this, 0, 0);
                }
                var segment = segments[0];
                return new Pointer(segment, 0, segment[0]);
            }
            set
            {
                if (SegmentCount != 0)
                {
                    value.WriteHeader(segments[0], 0);
                }
            }
        }

        private void Init(ISegmentFactory segmentFactory)
        {
            SegmentCount = 0;
            this.segmentFactory = segmentFactory;
        }

        void IRecyclable.Reset(bool recycling)
        {
            this.factoryState = null;
            this.segmentFactory = null;
        }
        public void Dispose()
        {
            Cache<Message>.Push(this);
        }
        private ISegmentFactory segmentFactory;
        private object factoryState;

        public int SegmentCount { get; private set; }

        public ISegment this[int index]
        {
            get
            {
                var tmp = segments;
                if (tmp == null) throw new ArgumentOutOfRangeException("index");
                return segments[index];
            }
        }

        public bool ReadNext() { return segmentFactory.ReadNext(this); }

        private ISegment[] segments;
        internal void ResetSegments(int max)
        {
            if (segments != null)
            {
                for (int i = 0; i < SegmentCount; i++)
                {
                    var seg = segments[i];
                    if (seg != null) seg.Reset(true);
                }
            }
            if (segments == null || max > segments.Length)
                Array.Resize(ref segments, Math.Min(max, DEFAULT_SIZE));
            SegmentCount = 0;
        }

        public ISegment ReuseExistingSegment()
        {
            return (segments != null && segments.Length > SegmentCount) ? segments[SegmentCount] : null;
        }
        const int DEFAULT_SIZE = 8;
        internal void AddSegment(ISegment segment)
        {
            if (segment == null) throw new ArgumentNullException("segment");

            if (segments == null)
            {
                segments = new ISegment[DEFAULT_SIZE];
            }
            else if (segments.Length == SegmentCount)
            {
                Array.Resize(ref segments, segments.Length + DEFAULT_SIZE);
            }
            segment.Init(this, SegmentCount);
            segments[SegmentCount++] = segment;
        }

        internal int Allocate(ref ISegment segment, ref bool plusHeader, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size");
            int count = SegmentCount;
            for (int i = (segment == null ? 0 : (segment.Index + 1)); i < count; i++)
            {
                int index;
                var tmp = this[i];
                if(plusHeader)
                {
                    if(tmp.TryAllocate(size + 1, out index))
                    {
                        segment = tmp;
                        return index;
                    }
                    plusHeader = false;
                }
                if (tmp.TryAllocate(size, out index))
                {
                    segment = tmp;
                    return index;
                }
            }
            // no existing segments can fit it
            segment = CreateSegment(plusHeader ? (size + 1) : size);
            return 0; // start at the start, then
        }

        private ISegment CreateSegment(int size)
        {
            ISegment seg = segmentFactory.TryAllocate(this, size);
            if (seg == null) throw new OutOfMemoryException(
                string.Format("Unable to allocate segment {0} with at least {1} words", SegmentCount, size));
            return seg;
        }

        public IEnumerator<ISegment> GetEnumerator()
        {
            int count = SegmentCount;
            for (int i = 0; i < count; i++)
                yield return this[i];
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void Write(Stream destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            var buffer = GetWriteBuffer();
            try
            {
                unchecked
                {
                    int bytes = WritePreamble(buffer);
                    if (bytes == 0) return;
                    destination.Write(buffer, 0, bytes);

                    int wordsInBuffer = buffer.Length >> 3;
                    foreach (var segment in this)
                    {
                        int words, offset = 0;
                        while ((words = WriteSegment(buffer, segment, offset)) != 0)
                        {
                            destination.Write(buffer, 0, words << 3);
                            offset += words;
                        }
                    }
                }
            }
            finally
            {
                FinishedWithWriteBuffer(buffer);
            }
        }
        public async void WriteAsync(Stream destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            var buffer = GetWriteBuffer();
            try
            {
                unchecked
                {
                    int bytes = WritePreamble(buffer);
                    if (bytes == 0) return;
                    await destination.WriteAsync(buffer, 0, bytes).ConfigureAwait(false);

                    int wordsInBuffer = buffer.Length >> 3;
                    foreach (var segment in this)
                    {
                        int words, offset = 0;
                        while ((words = WriteSegment(buffer, segment, offset)) != 0)
                        {
                            await destination.WriteAsync(buffer, 0, words << 3).ConfigureAwait(false);
                            offset += words;
                        }
                    }
                }
            }
            finally
            {
                FinishedWithWriteBuffer(buffer);
            }
        }

        private unsafe int WritePreamble(byte[] buffer)
        {
            int count = SegmentCount;
            if (count == 0) return 0;
            if ((count << 2) > buffer.Length)
                throw new InvalidOperationException("Not enough space in the buffer to write segment headers");
            int outputIndex = 0;
            fixed (byte* ptr = buffer)
            {
                int* headers = (int*)ptr;
                headers[outputIndex++] = count - 1;
                for (int i = 0; i < count; i++)
                    headers[outputIndex++] = this[i].Length;
                if ((count % 2) == 0) // need to add padding
                    headers[outputIndex++] = 0;
            }
            return outputIndex << 2;
        }
        private unsafe int WriteSegment(byte[] buffer, ISegment segment, int wordOffset)
        {
            int wordsInBuffer = buffer.Length >> 3;
            return segment.ReadWords(wordOffset, buffer, 0, wordsInBuffer);
        }

        [ThreadStatic]
        static byte[] pooledBuffer;
        private static byte[] GetWriteBuffer()
        {
            var buffer = pooledBuffer ?? new byte[WRITE_BUFFER_SIZE];
            pooledBuffer = null;
            return buffer;
        }
        private const int WRITE_BUFFER_SIZE = 1024 * 8;
        private static void FinishedWithWriteBuffer(byte[] buffer)
        {
            // make it available again
            if (pooledBuffer == null && buffer != null && buffer.Length == WRITE_BUFFER_SIZE)
            {
                Array.Clear(buffer, 0, buffer.Length);
                pooledBuffer = buffer;
            }
        }

        public T Allocate<T>()
        {
            return StructAccessor<T>.Instance.Create(this.Root);
        }
        public FixedSizeList<T> AllocateList<T>(int count)
        {
            return StructAccessor<T>.Instance.CreateList(this.Root, count);
        }
    }
}
