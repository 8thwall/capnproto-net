using System;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
#if FULLCLR
    [System.ComponentModel.ImmutableObject(true)]
#endif
    public sealed class StructAttribute: Attribute
    {
        public StructAttribute(ElementSize preferredSize, int dataWords = 0, int pointers = 0)
        {
            this.PreferredSize = preferredSize;
            this.DataWords = dataWords;
            this.Pointers = pointers;
        }
        public ElementSize PreferredSize { get; private set; }

        public int DataWords { get; private set; }
        public int Pointers { get; private set; }
    }
}
