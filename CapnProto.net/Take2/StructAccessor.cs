using System;
using System.Reflection;

namespace CapnProto.Take2
{
    internal abstract class StructAccessor<T>
    {
        public static readonly StructAccessor<T> Instance;

        static StructAccessor()
        {
            object tmp;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: tmp = new BooleanStructAccessor(); break;
                case TypeCode.Byte: tmp = new ByteStructAccessor(); break;
                case TypeCode.SByte: tmp = new SByteStructAccessor(); break;
                case TypeCode.UInt16: tmp = new UInt16StructAccessor(); break;
                case TypeCode.Int16: tmp = new Int16StructAccessor(); break;
                case TypeCode.UInt32: tmp = new UInt32StructAccessor(); break;
                case TypeCode.Int32: tmp = new Int32StructAccessor(); break;
                case TypeCode.UInt64: tmp = new UInt64StructAccessor(); break;
                case TypeCode.Int64: tmp = new Int64StructAccessor(); break;
                case TypeCode.Single: tmp = new SingleStructAccessor(); break;
                case TypeCode.Double: tmp = new DoubleStructAccessor(); break;
                default:
                    if(typeof(T) == typeof(Text))
                    {
                        tmp = new TextAccessor();
                    }
                    else if (typeof(T) == typeof(Data))
                    {
                        tmp = new DataAccessor();
                    }
                    else if (Attribute.IsDefined(typeof(T), typeof(GroupAttribute)))
                    {
                        tmp = new GroupStructAccessor<T>();
                    }
                    else
                    {
                        var @struct = (StructAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(StructAttribute));
                        if (@struct == null)
                        {
                            tmp = new MissingMetadataStructAccessor<T>();
                        }
                        else
                        {
                            checked
                            {
                                tmp = PointerStructAccessor<T>.Create(@struct.PreferredSize, (short)@struct.DataWords, (short)@struct.Pointers);
                            }
                        }
                    }
                    break;
            }
            Instance = (StructAccessor<T>)tmp;
        }
        public abstract T Get(Pointer pointer, int index);
        public abstract void Set(Pointer pointer, int index, T value);
        public abstract FixedSizeList<T> CreateList(Pointer pointer, int count);
        public abstract T Create(Pointer pointer);
    }
    internal abstract class BasicPointerAccessor<T> : StructAccessor<T>
    {
        public override FixedSizeList<T> CreateList(Pointer pointer, int count)
        {
            return (FixedSizeList<T>)pointer.AllocateList(ElementSize.EightBytesPointer, count);
        }
        public override T Create(Pointer pointer)
        {
            throw new NotSupportedException("Cannot be created in this way: " + typeof(T).FullName);
        }
    }
    internal class TextAccessor : BasicPointerAccessor<Text>
    {
        public override Text Get(Pointer pointer, int index)
        {
            return (Text)pointer.GetPointer(index);
        }
        public override void Set(Pointer pointer, int index, Text value)
        {
            pointer.SetPointer(index, value);
        }
    }
    internal class DataAccessor : BasicPointerAccessor<Data>
    {
        public override Data Get(Pointer pointer, int index)
        {
            return (Data)pointer.GetPointer(index);
        }
        public override void Set(Pointer pointer, int index, Data value)
        {
            pointer.SetPointer(index, value);
        }
    }
    abstract class BasicTypeAccessor<T> : StructAccessor<T>
    {
        public override T Create(Pointer pointer)
        {
            throw new NotSupportedException("Pointer to a non-pointer type is not supported: " + typeof(T).FullName);
        }
    }
    internal class BooleanStructAccessor : BasicTypeAccessor<bool>
    {
        public override bool Get(Pointer pointer, int index) { return pointer.GetListBoolean(index); }
        public override void Set(Pointer pointer, int index, bool value) { pointer.SetListBoolean(index, value); }
        public override FixedSizeList<bool> CreateList(Pointer pointer, int count) { return (FixedSizeList<bool>)pointer.AllocateList(ElementSize.OneBit, count); }
    }
    internal class ByteStructAccessor : BasicTypeAccessor<byte>
    {
        public override byte Get(Pointer pointer, int index) { return pointer.GetListByte(index); }
        public override void Set(Pointer pointer, int index, byte value) { pointer.SetListByte(index, value); }
        public override FixedSizeList<byte> CreateList(Pointer pointer, int count) { return (FixedSizeList<byte>)pointer.AllocateList(ElementSize.OneByte, count); }
    }
    internal class SByteStructAccessor : BasicTypeAccessor<sbyte>
    {
        public override sbyte Get(Pointer pointer, int index) { return pointer.GetListSByte(index); }
        public override void Set(Pointer pointer, int index, sbyte value) { pointer.SetListSByte(index, value); }
        public override FixedSizeList<sbyte> CreateList(Pointer pointer, int count) { return (FixedSizeList<sbyte>)pointer.AllocateList(ElementSize.OneByte, count); }
    }
    internal class UInt16StructAccessor : BasicTypeAccessor<ushort>
    {
        public override ushort Get(Pointer pointer, int index) { return pointer.GetListUInt16(index); }
        public override void Set(Pointer pointer, int index, ushort value) { pointer.SetListUInt16(index, value); }
        public override FixedSizeList<ushort> CreateList(Pointer pointer, int count) { return (FixedSizeList<ushort>)pointer.AllocateList(ElementSize.TwoBytes, count); }
    }
    internal class Int16StructAccessor : BasicTypeAccessor<short>
    {
        public override short Get(Pointer pointer, int index) { return pointer.GetListInt16(index); }
        public override void Set(Pointer pointer, int index, short value) { pointer.SetListInt16(index, value); }
        public override FixedSizeList<short> CreateList(Pointer pointer, int count) { return (FixedSizeList<short>)pointer.AllocateList(ElementSize.TwoBytes, count); }
    }
    internal class UInt32StructAccessor : BasicTypeAccessor<uint>
    {
        public override uint Get(Pointer pointer, int index) { return pointer.GetListUInt32(index); }
        public override void Set(Pointer pointer, int index, uint value) { pointer.SetListUInt32(index, value); }
        public override FixedSizeList<uint> CreateList(Pointer pointer, int count) { return (FixedSizeList<uint>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class Int32StructAccessor : BasicTypeAccessor<int>
    {
        public override int Get(Pointer pointer, int index) { return pointer.GetListInt32(index); }
        public override void Set(Pointer pointer, int index, int value) { pointer.SetListInt32(index, value); }
        public override FixedSizeList<int> CreateList(Pointer pointer, int count) { return (FixedSizeList<int>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class UInt64StructAccessor : BasicTypeAccessor<ulong>
    {
        public override ulong Get(Pointer pointer, int index) { return pointer.GetListUInt64(index); }
        public override void Set(Pointer pointer, int index, ulong value) { pointer.SetListUInt64(index, value); }
        public override FixedSizeList<ulong> CreateList(Pointer pointer, int count) { return (FixedSizeList<ulong>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }
    internal class Int64StructAccessor : BasicTypeAccessor<long>
    {
        public override long Get(Pointer pointer, int index) { return pointer.GetListInt64(index); }
        public override void Set(Pointer pointer, int index, long value) { pointer.SetListInt64(index, value); }
        public override FixedSizeList<long> CreateList(Pointer pointer, int count) { return (FixedSizeList<long>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }

    internal class SingleStructAccessor : BasicTypeAccessor<float>
    {
        public override float Get(Pointer pointer, int index) { return pointer.GetListSingle(index); }
        public override void Set(Pointer pointer, int index, float value) { pointer.SetListSingle(index, value);  }
        public override FixedSizeList<float> CreateList(Pointer pointer, int count) { return (FixedSizeList<float>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class DoubleStructAccessor : BasicTypeAccessor<double>
    {
        public override double Get(Pointer pointer, int index) { return pointer.GetListDouble(index); }
        public override void Set(Pointer pointer, int index, double value) { pointer.SetListDouble(index, value); }
        public override FixedSizeList<double> CreateList(Pointer pointer, int count) { return (FixedSizeList<double>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }
    internal abstract class FailStructAccessor<T> : StructAccessor<T>
    {
        protected abstract Exception Fail();
        public override T Get(Pointer pointer, int index) { throw Fail(); }
        public override void Set(Pointer pointer, int index, T value) { throw Fail(); }
        public override FixedSizeList<T> CreateList(Pointer pointer, int count) { throw Fail(); }
        public override T Create(Pointer pointer) { throw Fail(); }
    }
    internal class MissingMetadataStructAccessor<T> : FailStructAccessor<T>
    {
        protected override Exception Fail()
        {
            return new NotSupportedException("Type is missing StructAttribute metadata: " + typeof(T).FullName);
        }
    }
    internal class GroupStructAccessor<T> : FailStructAccessor<T>
    {
        protected override Exception Fail()
        {
            return new NotSupportedException("Type is a group (GroupAttribute), and cannot be accessed in this way: " + typeof(T).FullName);
        }
    }

    internal abstract class PointerStructAccessor<T> : StructAccessor<T>
    {
        public static PointerStructAccessor<T> Create(ElementSize elementSize, short dataWords, short pointers)
        {
            try
            {
                var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static);
                var op_toT = FindMethod(methods, typeof(Pointer), typeof(T), "op_implicit") ?? FindMethod(methods, typeof(Pointer), typeof(T), "op_explicit");
                var op_fromT = FindMethod(methods, typeof(T), typeof(Pointer), "op_implicit") ?? FindMethod(methods, typeof(T), typeof(Pointer), "op_explicit");

                if (op_toT != null && op_fromT != null)
                {
                    Func<Pointer, T> toT = (Func<Pointer, T>)Delegate.CreateDelegate(typeof(Func<Pointer, T>), null, op_toT);
                    Func<T, Pointer> fromT = (Func<T, Pointer>)Delegate.CreateDelegate(typeof(Func<T, Pointer>), null, op_fromT);
                    return new OperatorBasedStructAccessor(elementSize, dataWords, pointers, toT, fromT);
                }
            }
            catch { }
            return new DynamicPointerStructAccessor(elementSize, dataWords, pointers);
        }
        static MethodInfo FindMethod(MethodInfo[] methods, Type from, Type to, string name)
        {
            for (int i = 0; i < methods.Length; i++)
            {
                var method = methods[i];
                if (method.IsPublic && method.IsStatic &&
                    string.Equals(name, method.Name, StringComparison.CurrentCultureIgnoreCase) && method.ReturnType == to)
                {
                    var args = method.GetParameters();
                    if (args.Length == 1 && args[0].ParameterType == from) return method;
                }
            }
            return null;
        }
        private PointerStructAccessor(ElementSize elementSize, short dataWords, short pointers)
        {
            this.elementSize = elementSize;
            this.dataWords = dataWords;
            this.pointers = pointers;
        }
        private readonly ElementSize elementSize;
        private readonly short dataWords, pointers;
        protected Pointer CreateListImpl(Pointer pointer, int count)
        {
            if (elementSize == ElementSize.InlineComposite)
            {
                return pointer.AllocateList(dataWords, pointers, count);
            }
            else
            {
                return pointer.AllocateList(elementSize, count);
            }
        }
        protected Pointer CreateImpl(Pointer pointer)
        {
            return pointer.Allocate(dataWords, pointers);
        }
        public override FixedSizeList<T> CreateList(Pointer pointer, int count)
        {
            return (FixedSizeList<T>)CreateListImpl(pointer, count);
        }

        private class OperatorBasedStructAccessor : PointerStructAccessor<T>
        {
            private readonly Func<Pointer, T> toT;
            private readonly Func<T, Pointer> fromT;
            public OperatorBasedStructAccessor(ElementSize elementSize, short dataWords, short pointers,
                Func<Pointer, T> toT, Func<T, Pointer> fromT)
                : base(elementSize, dataWords, pointers)
            {
                this.toT = toT;
                this.fromT = fromT;
            }
            public override T Create(Pointer pointer)
            {
                return toT(CreateImpl(pointer));
            }
            public override T Get(Pointer pointer, int index)
            {
                return toT(pointer.GetPointer(index));
            }
            public override void Set(Pointer pointer, int index, T value)
            {
                pointer.SetPointer(index, fromT(value));
            }
        }

        // THIS BOXES; it is just a lazy fallback, but it comes up with appropriate errors, at least
        private class DynamicPointerStructAccessor : PointerStructAccessor<T>
        {
            public DynamicPointerStructAccessor(ElementSize elementSize, short dataWords, short pointers)
                : base(elementSize, dataWords, pointers)
            { }
            public override T Get(Pointer pointer, int index)
            {
                return (T)(dynamic)pointer.GetPointer(index);
            }

            public override void Set(Pointer pointer, int index, T value)
            {
                pointer.SetPointer(index, (Pointer)(dynamic)value);
            }
            public override T Create(Pointer pointer)
            {
                return (T)(dynamic)CreateImpl(pointer);
            }
        }
    }
}
