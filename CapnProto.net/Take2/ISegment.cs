
namespace CapnProto.Take2
{
    public interface ISegment : IRecyclable
    {
        int Length { get; }
        int Index { get; }
        Message Message { get; }
        ulong this[int index] { get; set; }
        void SetValue(int index, ulong value, ulong mask);

        void Init(Message message, int index);

        bool TryAllocate(int words, out int index);
    }
}
