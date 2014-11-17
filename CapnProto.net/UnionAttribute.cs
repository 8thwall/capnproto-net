using System;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
#if FULLCLR
    [System.ComponentModel.ImmutableObject(true)]
#endif
    public sealed class UnionAttribute : Attribute
    {
        public UnionAttribute(int tag, int start = -1, int end = -1)
        {
            Tag = tag;
            Start = start;
            End = end;
        }
        public int Tag { get; private set; }

        public int Start  { get; private set; }
        public int End { get; private set; }
    }
}
