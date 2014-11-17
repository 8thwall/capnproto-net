using System;
using System.ComponentModel;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
#if !PCL
    [ImmutableObject(true)]
#endif
    public sealed class FieldAttribute : Attribute
    {
        public FieldAttribute(int number, int start = -1, int end = -1, int pointer = -1)
        {
            this.Number= number;
            Start = start;
            End = end;
            Pointer = pointer;
        }
        public int Number { get; private set; }
        public int Start { get; private set; }
        public int End { get; private set; }
        public int Pointer { get; private set; }
    }
}
