
namespace CapnProto
{
    public static class Segments
    {
        public static string ToString(this ISegment segment, int wordIndex)
        {
            return ToString(segment[wordIndex]);
        }
        public static string ToString(ulong value)
        {
            unchecked
            {
                return string.Format("{0:X2}{1:X2}-{2:X2}{3:X2}-{4:X2}{5:X2}-{6:X2}{7:X2}",
                    (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
                    (byte)(value >> 32), (byte)(value >> 40), (byte)(value >> 48), (byte)(value >> 56)
                );
            }
        }
    }
}
