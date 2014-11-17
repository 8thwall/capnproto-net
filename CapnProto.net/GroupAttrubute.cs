using System;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
#if FULLCLR
    [System.ComponentModel.ImmutableObject(true)]
#endif
    public sealed class GroupAttribute : Attribute
    {
    }
}
