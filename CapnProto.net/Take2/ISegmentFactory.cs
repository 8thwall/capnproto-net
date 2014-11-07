
namespace CapnProto.Take2
{
    public interface ISegmentFactory
    {

        bool ReadNext(object state, Message message);
    }
}
