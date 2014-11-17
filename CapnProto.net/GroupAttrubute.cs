using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace CapnProto
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
#if !PCL
    [ImmutableObject(true)]
#endif
    public sealed class GroupAttribute : Attribute
    {
    }
}
