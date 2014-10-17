using System;
using System.Collections;

namespace CapnProto
{
    public abstract class TypeModel
    {
        public abstract ITypeSerializer GetSerializer(Type type);
        public ITypeSerializer<T> GetSerializer<T>()
        {
            return (ITypeSerializer<T>)GetSerializer(typeof(T));
        }
    }

    public sealed class RuntimeTypeModel : TypeModel
    {
        private static readonly RuntimeTypeModel @default = new RuntimeTypeModel();
        public static TypeModel Default { get { return @default; } }

        readonly Hashtable typeSerializers = new Hashtable();

        private ITypeSerializer CreateSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            throw new NotImplementedException();
        }
        public override ITypeSerializer GetSerializer(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            ITypeSerializer ser = (ITypeSerializer)typeSerializers[type];
            if (ser == null)
            {
                ser = CreateSerializer(type);
                lock (typeSerializers)
                {
                    typeSerializers[type] = ser;
                }
            }
            return ser;
        }
    }
}
