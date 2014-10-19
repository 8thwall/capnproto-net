
namespace CapnProto
{
    public interface IBlittable
    {
        void Deserialize(int segment, int origin, global::CapnProto.CapnProtoReader reader, ulong pointer);
    }
}
