
namespace CapnProto
{
    public interface ISegmentFactory
    {

        bool ReadNext(Message message);

        ISegment TryAllocate(Message message, int words);
    }
}
