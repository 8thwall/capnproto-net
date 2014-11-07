
namespace CapnProto.Take2
{
    public abstract class Segment : ISegment
    {
        public int Index
        {
            get { return index; }
        }
        private Message message;
        private int index;
        public Message Message
        {
            get { return message; }
        }

        public abstract ulong this[int index] { get; set; }

        public void Init(Message message, int index)
        {
            this.message = message;
            this.index = index;
        }

        public virtual bool TryAllocate(int size, out int index)
        {
            index = int.MinValue;
            return false;
        }

        public virtual void Reset(bool recycling)
        {
            message = null;
        }
    }
}
