
namespace CapnProto
{
    public interface IPointer
    {
        Pointer Pointer { get; }
    }
    public static class Pointers
    {
        public static bool IsValid<T>(this T pointer) where T : struct, IPointer
        {
            return pointer.Pointer.IsValid;
        }
    }
}
