using System;
using System.ComponentModel;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UnionAttribute : Attribute
    {
        public UnionAttribute(int tag)
        {
            Tag = tag;
            Offset = Pointer = -1;
        }
        public int Tag { get; private set; }

        public int Offset { get; set; }
        public int Pointer { get; set; }
    }
}
