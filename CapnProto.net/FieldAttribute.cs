using System;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FieldAttribute : Attribute
    {
        public FieldAttribute(int number)
        {
            this.Number= number;
        }
        public int Number { get; private set; }
        public int Offset { get; set; }
    }
}
