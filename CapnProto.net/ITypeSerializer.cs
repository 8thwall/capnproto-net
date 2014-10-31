
using System;

namespace CapnProto
{
    public sealed class DeserializationContext
    {
        private readonly TypeModel model;
        private readonly CapnProtoReader reader;
        private readonly object state;
        public TypeModel Model {  get { return model; } }
        public CapnProtoReader Reader { get { return reader; } }
        public object State { get { return state; } }
        public DeserializationContext(TypeModel model, CapnProtoReader reader, object state)
        {
            this.model = model;
            this.reader = reader;
            this.state = state;
        }
    }
    public interface ITypeSerializer
    {
        TypeModel Model { get; }
        object Deserialize(int segment, int origin, DeserializationContext ctx, ulong pointer);
    }
    public interface ITypeSerializer<T> : ITypeSerializer
    {
        new T Deserialize(int segment, int origin, DeserializationContext ctx, ulong pointer);
    }
}
