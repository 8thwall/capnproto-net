
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

        int WriteString(int index, string value, int bytes);
        string ReadString(int index, int bytes);

        int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords);

        int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords);
    }
}
