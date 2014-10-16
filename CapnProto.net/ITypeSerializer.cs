
namespace CapnProto
{
    public interface ITypeSerializer
    {
        object Deserialize(CapnProtoReader reader);
    }
    public interface ITypeSerializer<T> : ITypeSerializer
    {
        new T Deserialize(CapnProtoReader reader);
    }
}
