
using System;
using System.Collections.Generic;
namespace CapnProto.Take2
{
    public sealed class Message : IDisposable, IRecyclable, ISegment, IEnumerable<ISegment>
    {
        public const int WordLength = 8;
        private Message() { }

        public object FactoryState { get { return factoryState; } }
        public static Message Create(object factoryState, ISegmentFactory segmentFactory)
        {
            var msg = Cache<Message>.Pop() ?? new Message();
            msg.Init(factoryState, segmentFactory);
            return msg;
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
                    return new Pointer(this, 0);
                }
                return new Pointer(segments[0], 0);
            }
            set
            {
                if (SegmentCount != 0)
                {
                    value.WriteHeader(segments[0], 0);
                }
            }
        }

        private void Init(object factoryState, ISegmentFactory segmentFactory)
        {
            SegmentCount = 0;
            this.factoryState = factoryState;
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

        internal int Allocate(ref int segment, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size");
            for (int i = segment; i < SegmentCount; i++)
            {
                int index;
                if (this[i].TryAllocate(size, out index))
                {
                    segment = i;
                    return index;
                }
            }
            ISegment seg = CreateSegment(size);
            return 0; // start at the start, then
        }

        private ISegment CreateSegment(int size)
        {
            ISegment seg = segmentFactory.TryAllocate(this, size);
            if (seg == null) throw new OutOfMemoryException(
                string.Format("Unable to allocate segment {0} with at least {1} words", SegmentCount, size));
            return seg;
        }

        IEnumerator<ISegment> IEnumerable<ISegment>.GetEnumerator()
        {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        private IEnumerator<ISegment> GetEnumerator()
        {
            int count = SegmentCount;
            for (int i = 0; i < count; i++)
                yield return this[i];
        }        
    }
}
