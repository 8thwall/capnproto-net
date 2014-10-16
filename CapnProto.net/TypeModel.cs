using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapnProto
{
    public class TypeModel
    {
        public static TypeModel Default { get { return @default; } }
        private static readonly TypeModel @default = new TypeModel();

        readonly Hashtable typeSerializers = new Hashtable();
        public ITypeSerializer GetSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            ITypeSerializer ser = (ITypeSerializer)typeSerializers[type];
            if(ser == null)
            {
                ser = CreateSerializer(type);
                lock(typeSerializers)
                {
                    typeSerializers[type] = ser;
                }
            }
            return ser;
        }

        public ITypeSerializer<T> GetSerializer<T>()
        {
            return (ITypeSerializer<T>)GetSerializer(typeof(T));
        }

        internal ITypeSerializer CreateSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");


            throw new NotImplementedException();
        }
    }
}
