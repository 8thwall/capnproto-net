
namespace CapnProto
{
    public interface IBlittable
    {
        void Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer);
    }
}
