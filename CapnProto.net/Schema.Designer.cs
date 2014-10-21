using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    static partial class Schema
    {
        partial class Type
        {
            internal const int LEN_POINTER = -1;
            public int GetFieldLength()
            {
#warning Redo when the enum discriminator is working
                if (@void != null) return 0;
                if (@bool != null) return 1;
                if (int8 != null || uint8 != null) return 8;
                if ((int16 ?? uint16) != null || @enum != null) return 16;
                if ((int32 ?? uint32 ?? float32) != null) return 32;
                if ((int64 ?? uint64 ?? float64) != null) return 64;
                if ((data ?? text ?? anyPointer) != null || list != null || @struct != null) return LEN_POINTER;
                return 0;
            }
        }

        partial class Node
        {
            public override string ToString()
            {
                return id == 0 ? displayName : (id + ": " + displayName);
            }
            internal string CustomSerializerName()
            {
                return CodeWriter.PrivatePrefix + "r_" + UniqueName();
            }
            internal string UniqueName()
            {
                return Convert.ToString(unchecked((long)id), 16);
            }
        }


        
    }
}
