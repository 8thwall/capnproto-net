
namespace CapnProto.Take2
{
    public interface ISegmentFactory
    {

        bool ReadNext(Message message);

        ISegment TryAllocate(Message message, int size);
    }
}
