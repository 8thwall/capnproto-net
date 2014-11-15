using System;
using System.Reflection;

namespace CapnProto
{
    internal abstract class TypeAccessor<T>
    {
        public static readonly TypeAccessor<T> Instance;
        public abstract bool IsStruct { get; }
        public abstract bool IsPointer { get; }
        static TypeAccessor()
        {
            object tmp;
            switch (Type.GetTypeCode(typeof(T)))
            {
                case TypeCode.Boolean: tmp = new BooleanAccessor(); break;
                case TypeCode.Byte: tmp = new ByteAccessor(); break;
                case TypeCode.SByte: tmp = new SByteAccessor(); break;
                case TypeCode.UInt16: tmp = new UInt16Accessor(); break;
                case TypeCode.Int16: tmp = new Int16Accessor(); break;
                case TypeCode.UInt32: tmp = new UInt32Accessor(); break;
                case TypeCode.Int32: tmp = new Int32Accessor(); break;
                case TypeCode.UInt64: tmp = new UInt64Accessor(); break;
                case TypeCode.Int64: tmp = new Int64Accessor(); break;
                case TypeCode.Single: tmp = new SingleAccessor(); break;
                case TypeCode.Double: tmp = new DoubleAccessor(); break;
                default:
                    if(typeof(T) == typeof(Text))
                    {
                        tmp = new TextAccessor();
                    }
                    else if (typeof(T) == typeof(Data))
                    {
                        tmp = new DataAccessor();
                    }
                    else if (typeof(T) == typeof(Pointer))
                    {
                        tmp = new PointerAccessor();
                    }
                    else if (Attribute.IsDefined(typeof(T), typeof(GroupAttribute)))
                    {
                        tmp = new GroupAccessor<T>();
                    }
                    else
                    {
                        var @struct = (StructAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(StructAttribute));
                        if (@struct == null)
                        {
                            if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(FixedSizeList<>))
                            {
                                tmp = Activator.CreateInstance(
                                    typeof(FixedSizeListAccessor<>).MakeGenericType(typeof(T).GetGenericArguments()));
                            }
                            else
                            {
                                tmp = new MissingMetadataAccessor<T>();
                            }
                        }
                        else
                        {
                            checked
                            {
                                tmp = StructAccessor<T>.Create(@struct.PreferredSize, (short)@struct.DataWords, (short)@struct.Pointers);
                            }
                        }
                    }
                    break;
            }
            Instance = (TypeAccessor<T>)tmp;
        }
        public abstract T GetElement(Pointer pointer, int index);
        public abstract void SetElement(Pointer pointer, int index, T value);
        public abstract FixedSizeList<T> CreateList(Pointer pointer, int count);
        public abstract T Create(Pointer pointer);

        public virtual FixedSizeList<T> CreateList(Pointer pointer, int count, ElementSize elementSize)
        {
            throw new NotImplementedException();
        }
    }
    internal abstract class BasicPointerAccessor<T> : TypeAccessor<T>
    {
        public override bool IsPointer
        {
            get { return true; }
        }
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
        public override bool IsStruct
        {
            get { return false; }
        }
        public override Text GetElement(Pointer pointer, int index)
        {
            return (Text)pointer.GetListPointer(index);
        }
        public override void SetElement(Pointer pointer, int index, Text value)
        {
            pointer.SetListPointer(index, value);
        }
    }
    internal class DataAccessor : BasicPointerAccessor<Data>
    {
        public override bool IsStruct
        {
            get { return false; }
        }
        public override Data GetElement(Pointer pointer, int index)
        {
            return (Data)pointer.GetListPointer(index);
        }
        public override void SetElement(Pointer pointer, int index, Data value)
        {
            pointer.SetListPointer(index, value);
        }
    }
    abstract class BasicTypeAccessor<T> : TypeAccessor<T>
    {
        public override bool IsPointer
        {
            get { return false; }
        }
        public override bool IsStruct
        {
            get { return false; }
        }
        public override T Create(Pointer pointer)
        {
            throw new NotSupportedException("Pointer to a non-pointer type is not supported: " + typeof(T).FullName);
        }
    }
    internal class BooleanAccessor : BasicTypeAccessor<bool>
    {
        public override bool GetElement(Pointer pointer, int index) { return pointer.GetListBoolean(index); }
        public override void SetElement(Pointer pointer, int index, bool value) { pointer.SetListBoolean(index, value); }
        public override FixedSizeList<bool> CreateList(Pointer pointer, int count) { return (FixedSizeList<bool>)pointer.AllocateList(ElementSize.OneBit, count); }
    }
    internal class ByteAccessor : BasicTypeAccessor<byte>
    {
        public override byte GetElement(Pointer pointer, int index) { return pointer.GetListByte(index); }
        public override void SetElement(Pointer pointer, int index, byte value) { pointer.SetListByte(index, value); }
        public override FixedSizeList<byte> CreateList(Pointer pointer, int count) { return (FixedSizeList<byte>)pointer.AllocateList(ElementSize.OneByte, count); }
    }
    internal class SByteAccessor : BasicTypeAccessor<sbyte>
    {
        public override sbyte GetElement(Pointer pointer, int index) { return pointer.GetListSByte(index); }
        public override void SetElement(Pointer pointer, int index, sbyte value) { pointer.SetListSByte(index, value); }
        public override FixedSizeList<sbyte> CreateList(Pointer pointer, int count) { return (FixedSizeList<sbyte>)pointer.AllocateList(ElementSize.OneByte, count); }
    }
    internal class UInt16Accessor : BasicTypeAccessor<ushort>
    {
        public override ushort GetElement(Pointer pointer, int index) { return pointer.GetListUInt16(index); }
        public override void SetElement(Pointer pointer, int index, ushort value) { pointer.SetListUInt16(index, value); }
        public override FixedSizeList<ushort> CreateList(Pointer pointer, int count) { return (FixedSizeList<ushort>)pointer.AllocateList(ElementSize.TwoBytes, count); }
    }
    internal class Int16Accessor : BasicTypeAccessor<short>
    {
        public override short GetElement(Pointer pointer, int index) { return pointer.GetListInt16(index); }
        public override void SetElement(Pointer pointer, int index, short value) { pointer.SetListInt16(index, value); }
        public override FixedSizeList<short> CreateList(Pointer pointer, int count) { return (FixedSizeList<short>)pointer.AllocateList(ElementSize.TwoBytes, count); }
    }
    internal class UInt32Accessor : BasicTypeAccessor<uint>
    {
        public override uint GetElement(Pointer pointer, int index) { return pointer.GetListUInt32(index); }
        public override void SetElement(Pointer pointer, int index, uint value) { pointer.SetListUInt32(index, value); }
        public override FixedSizeList<uint> CreateList(Pointer pointer, int count) { return (FixedSizeList<uint>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class Int32Accessor : BasicTypeAccessor<int>
    {
        public override int GetElement(Pointer pointer, int index) { return pointer.GetListInt32(index); }
        public override void SetElement(Pointer pointer, int index, int value) { pointer.SetListInt32(index, value); }
        public override FixedSizeList<int> CreateList(Pointer pointer, int count) { return (FixedSizeList<int>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class UInt64Accessor : BasicTypeAccessor<ulong>
    {
        public override ulong GetElement(Pointer pointer, int index) { return pointer.GetListUInt64(index); }
        public override void SetElement(Pointer pointer, int index, ulong value) { pointer.SetListUInt64(index, value); }
        public override FixedSizeList<ulong> CreateList(Pointer pointer, int count) { return (FixedSizeList<ulong>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }
    internal class Int64Accessor : BasicTypeAccessor<long>
    {
        public override long GetElement(Pointer pointer, int index) { return pointer.GetListInt64(index); }
        public override void SetElement(Pointer pointer, int index, long value) { pointer.SetListInt64(index, value); }
        public override FixedSizeList<long> CreateList(Pointer pointer, int count) { return (FixedSizeList<long>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }

    internal class SingleAccessor : BasicTypeAccessor<float>
    {
        public override float GetElement(Pointer pointer, int index) { return pointer.GetListSingle(index); }
        public override void SetElement(Pointer pointer, int index, float value) { pointer.SetListSingle(index, value);  }
        public override FixedSizeList<float> CreateList(Pointer pointer, int count) { return (FixedSizeList<float>)pointer.AllocateList(ElementSize.FourBytes, count); }
    }
    internal class DoubleAccessor : BasicTypeAccessor<double>
    {
        public override double GetElement(Pointer pointer, int index) { return pointer.GetListDouble(index); }
        public override void SetElement(Pointer pointer, int index, double value) { pointer.SetListDouble(index, value); }
        public override FixedSizeList<double> CreateList(Pointer pointer, int count) { return (FixedSizeList<double>)pointer.AllocateList(ElementSize.EightBytesNonPointer, count); }
    }

    internal class PointerAccessor : FailTypeAccessor<Pointer>
    {
        protected override Exception Fail()
        {
            return new InvalidOperationException("Untyped pointers cannot be used in this way");
        }
        public override bool IsPointer
        {
            get { return true; }
        }
    }
    internal abstract class FailTypeAccessor<T> : TypeAccessor<T>
    {
        public override bool IsStruct
        {
            get { return false; }
        }
        public override bool IsPointer
        {
            get { return false; }
        }
        protected abstract Exception Fail();
        public override T GetElement(Pointer pointer, int index) { throw Fail(); }
        public override void SetElement(Pointer pointer, int index, T value) { throw Fail(); }
        public override FixedSizeList<T> CreateList(Pointer pointer, int count) { throw Fail(); }
        public override T Create(Pointer pointer) { throw Fail(); }
    }
    internal class MissingMetadataAccessor<T> : FailTypeAccessor<T>
    {
        protected override Exception Fail()
        {
            return new NotSupportedException("Type is missing StructAttribute metadata: " + typeof(T).FullName);
        }
    }
    internal class GroupAccessor<T> : FailTypeAccessor<T>
    {
        protected override Exception Fail()
        {
            return new NotSupportedException("Type is a group (GroupAttribute), and cannot be accessed in this way: " + typeof(T).FullName);
        }
    }
    internal class FixedSizeListAccessor<TInner> : BasicPointerAccessor<FixedSizeList<TInner>>
    {
        public override bool IsStruct
        {
            get { return false; }
        }
        public override FixedSizeList<TInner> GetElement(Pointer pointer, int index)
        {
            return (FixedSizeList<TInner>)pointer.GetListPointer(index);
        }
        public override void SetElement(Pointer pointer, int index, FixedSizeList<TInner> value)
        {
            pointer.SetListPointer(index, value);
        }
        public override FixedSizeList<FixedSizeList<TInner>> CreateList(Pointer pointer, int count)
        {
            return (FixedSizeList<FixedSizeList<TInner>>)pointer.AllocateList(ElementSize.EightBytesPointer, count);
        }
        public override FixedSizeList<FixedSizeList<TInner>> CreateList(Pointer pointer, int count, ElementSize elementSize)
        {
            if (elementSize != ElementSize.EightBytesPointer) throw new ArgumentException("Must be created as a list of size " + ElementSize.EightBytesPointer, "elementSize");
            return CreateList(pointer, count);
        }
        
    }
    internal abstract class StructAccessor<T> : TypeAccessor<T>
    {
        public override bool IsPointer
        {
            get { return true; }
        }
        public override void SetElement(Pointer pointer, int index, T value)
        {
            throw new NotSupportedException();
        }
        public static StructAccessor<T> Create(ElementSize elementSize, short dataWords, short pointers)
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
                    return new OperatorBasedStructTypeAccessor(elementSize, dataWords, pointers, toT, fromT);
                }
            }
            catch { }
            return new DynamicStructTypeAccessor(elementSize, dataWords, pointers);
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
        private StructAccessor(ElementSize elementSize, short dataWords, short pointers)
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

        private class OperatorBasedStructTypeAccessor : StructAccessor<T>
        {
            public override bool IsStruct
            {
                get { return true; }
            }
            private readonly Func<Pointer, T> toT;
            private readonly Func<T, Pointer> fromT;
            public OperatorBasedStructTypeAccessor(ElementSize elementSize, short dataWords, short pointers,
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
            public override T GetElement(Pointer pointer, int index)
            {
                return toT(pointer.GetListStruct(index));
            }
            public override void SetElement(Pointer pointer, int index, T value)
            {
                if (fromT(value) != pointer.GetListStruct(index))
                {
                    throw new NotSupportedException();
                }
                // otherwise: no harm, no foul
            }
        }


        // THIS BOXES; it is just a lazy fallback, but it comes up with appropriate errors, at least
        private class DynamicStructTypeAccessor : StructAccessor<T>
        {
            public override bool IsStruct
            {
                get { return true; }
            }
            public DynamicStructTypeAccessor(ElementSize elementSize, short dataWords, short pointers)
                : base(elementSize, dataWords, pointers)
            { }
            public override T GetElement(Pointer pointer, int index)
            {
                return (T)(dynamic)pointer.GetListStruct(index);
            }
            public override T Create(Pointer pointer)
            {
                return (T)(dynamic)CreateImpl(pointer);
            }
            public override void SetElement(Pointer pointer, int index, T value)
            {
                if (((IPointer)value).Pointer != pointer.GetListStruct(index))
                {
                    throw new NotSupportedException();
                }
                // otherwise: no harm, no foul
            }

        }
    }
}
