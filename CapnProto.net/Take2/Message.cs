
using System;
namespace CapnProto.Take2
{
    public sealed class Message : IDisposable, IRecyclable
    {
        private Message() { }

        public static Message Create(object state, ISegmentFactory segmentFactory)
        {
            var msg = Cache<Message>.Pop() ?? new Message();
            msg.Init(state, segmentFactory);
            return msg;
        }

        public AnyPointer Root
        {
            get { return new Pointer(segments[0], 0, 0); }
        }

        private void Init(object state, ISegmentFactory segmentFactory)
        {
            SegmentCount = 0;
            this.state = state;
            this.segmentFactory = segmentFactory;            
        }

        void IRecyclable.Reset(bool recycling)
        {
            this.state = null;
            this.segmentFactory = null;
        }
        public void Dispose()
        {
            Cache<Message>.Push(this);
        }
        private ISegmentFactory segmentFactory;
        private object state;

        public int SegmentCount { get; private set; }

        public ISegment this[int index] { get { throw new NotImplementedException(); } }

        public bool ReadNext() { return segmentFactory.ReadNext(state, this); }

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
    }
}
