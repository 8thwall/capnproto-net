
namespace CapnProto.Take2
{
    public interface ISegment : IRecyclable
    {
        int Index { get; }
        Message Message { get; }
        ulong this[int index] { get; set; }

        void Init(Message message, int index);

        bool TryAllocate(int size, out int index);
    }
}
