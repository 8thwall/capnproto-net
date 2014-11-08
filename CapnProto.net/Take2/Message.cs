
using System;
namespace CapnProto.Take2
{
    public sealed class Message : IDisposable, IRecyclable
    {
        private Message() { }

        public object FactoryState { get { return factoryState; } }
        public static Message Create(object factoryState, ISegmentFactory segmentFactory)
        {
            var msg = Cache<Message>.Pop() ?? new Message();
            msg.Init(factoryState, segmentFactory);
            return msg;
        }

        public Pointer Root
        {
            get { return new Pointer(segments[0], 0, 0); }
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

        public ISegment this[int index] { get { throw new NotImplementedException(); } }

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
            else if(segments.Length == SegmentCount)
            {
                Array.Resize(ref segments, segments.Length + DEFAULT_SIZE);
            }
            segment.Init(this, SegmentCount);
            segments[SegmentCount++] = segment;
        }

        internal int Allocate(ref int segment, int size)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException("size");
            for(int i = segment; i < SegmentCount ; i++)
            {
                int index;
                if(this[i].TryAllocate(size, out index))
                {
                    segment = i;
                    return index;
                }
            }
            ISegment seg = segmentFactory.TryAllocate(this, size);
            if (seg != null)
            {
                segment = seg.Index;
                return 0; // start at the start, then
            }
            throw new OutOfMemoryException("Unable to allocate block");
        }
    }
}
