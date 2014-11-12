
using System;
namespace CapnProto
{
    public interface ISegmentFactory : IDisposable
    {

        bool ReadNext(Message message);

        ISegment TryAllocate(Message message, int words);
    }
}
