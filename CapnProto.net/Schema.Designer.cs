using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    static partial class Schema
    {
        partial class Node
        {
            public override string ToString()
            {
                return id == 0 ? displayName : (id + ": " + displayName);
            }
        }
    }
}
