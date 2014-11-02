
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

        int depth = 0;
        const int MAX_DEPTH = 50;
        public int Depth { get { return depth; } }
        public void StepIn()
        {
            if (++depth > MAX_DEPTH)
                throw new InvalidOperationException(string.Format("Maximum object depth {0} exceeded", MAX_DEPTH));
        }
        public void StepOut()
        {
            depth--;
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
