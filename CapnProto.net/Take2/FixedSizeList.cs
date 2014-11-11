
using System;
using System.Collections.Generic;
namespace CapnProto.Take2
{
    internal abstract class StructAccessor<T>
    {
        public static readonly StructAccessor<T> Instance;

        static StructAccessor()
        {
            object tmp;
            switch(Type.GetTypeCode(typeof(T)))
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
                    if (Attribute.IsDefined(typeof(T), typeof(GroupAttribute)))
                    {
                        tmp = new GroupStructAccessor<T>();
                    }
                    else
                    {
                        var @struct = (StructAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(StructAttribute));
                        if(@struct == null)
                        {
                            tmp = new MissingMetadataStructAccessor<T>();
                        } else {
                            checked
                            {
                                tmp = new DynamicPointerStructAccessor<T>(@struct.PreferredSize, (short)@struct.DataWords, (short)@struct.Pointers);
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
    abstract class BasicTypeAccessor<T> : StructAccessor<T>
    {
        public override T Create(Pointer pointer)
        {
            throw new NotSupportedException("Pointer to a non-pointer type is not supported: " + typeof(T).FullName);
        }
    }
    internal class BooleanStructAccessor : BasicTypeAccessor<bool>
    {
        public override bool Get(Pointer pointer, int index) { return (pointer.GetDataWord(index) & 1) == 1; }
        public override void Set(Pointer pointer, int index, bool value) { pointer.SetDataWord(index, value ? (ulong)1 : (ulong)0, (ulong)0x01); }
        public override FixedSizeList<bool> CreateList(Pointer pointer, int count) { return (FixedSizeList<bool>)pointer.Allocate(ElementSize.OneBit, count); }
    }
    internal class ByteStructAccessor : BasicTypeAccessor<byte>
    {
        public override byte Get(Pointer pointer, int index) { return unchecked((byte)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, byte value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFF); }
        public override FixedSizeList<byte> CreateList(Pointer pointer, int count) { return (FixedSizeList<byte>)pointer.Allocate(ElementSize.OneByte, count); }
    }
    internal class SByteStructAccessor : BasicTypeAccessor<sbyte>
    {
        public override sbyte Get(Pointer pointer, int index) { return unchecked((sbyte)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, sbyte value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFF); }
        public override FixedSizeList<sbyte> CreateList(Pointer pointer, int count) { return (FixedSizeList<sbyte>)pointer.Allocate(ElementSize.OneByte, count); }
    }
    internal class UInt16StructAccessor : BasicTypeAccessor<ushort>
    {
        public override ushort Get(Pointer pointer, int index) { return unchecked((ushort)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, ushort value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFFFF); }
        public override FixedSizeList<ushort> CreateList(Pointer pointer, int count) { return (FixedSizeList<ushort>)pointer.Allocate(ElementSize.TwoBytes, count); }
    }
    internal class Int16StructAccessor : BasicTypeAccessor<short>
    {
        public override short Get(Pointer pointer, int index) { return unchecked((short)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, short value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFFFF); }
        public override FixedSizeList<short> CreateList(Pointer pointer, int count) { return (FixedSizeList<short>)pointer.Allocate(ElementSize.TwoBytes, count); }
    }
    internal class UInt32StructAccessor : BasicTypeAccessor<uint>
    {
        public override uint Get(Pointer pointer, int index) { return unchecked((uint)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, uint value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFFFFFFFF); }
        public override FixedSizeList<uint> CreateList(Pointer pointer, int count) { return (FixedSizeList<uint>)pointer.Allocate(ElementSize.FourBytes, count); }
    }
    internal class Int32StructAccessor : BasicTypeAccessor<int>
    {
        public override int Get(Pointer pointer, int index) { return unchecked((int)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, int value) { pointer.SetDataWord(index, unchecked((ulong)value), (ulong)0xFFFFFFFF); }
        public override FixedSizeList<int> CreateList(Pointer pointer, int count) { return (FixedSizeList<int>)pointer.Allocate(ElementSize.FourBytes, count); }
    }
    internal class UInt64StructAccessor : BasicTypeAccessor<ulong>
    {
        public override ulong Get(Pointer pointer, int index) { return pointer.GetDataWord(index); }
        public override void Set(Pointer pointer, int index, ulong value) { pointer.SetDataWord(index, value, ~(ulong)0); }
        public override FixedSizeList<ulong> CreateList(Pointer pointer, int count) { return (FixedSizeList<ulong>)pointer.Allocate(ElementSize.EightBytesNonPointer, count); }
    }
    internal class Int64StructAccessor : BasicTypeAccessor<long>
    {
        public override long Get(Pointer pointer, int index) { return unchecked((long)pointer.GetDataWord(index)); }
        public override void Set(Pointer pointer, int index, long value) { pointer.SetDataWord(index, unchecked((ulong)value), ~(ulong)0); }
        public override FixedSizeList<long> CreateList(Pointer pointer, int count) { return (FixedSizeList<long>)pointer.Allocate(ElementSize.EightBytesNonPointer, count); }
    }

    internal class SingleStructAccessor : BasicTypeAccessor<float>
    {
        public override unsafe float Get(Pointer pointer, int index)
        {
            uint value = unchecked((uint)pointer.GetDataWord(index));
            return *(float*)(&value);
        }
        public override unsafe void Set(Pointer pointer, int index, float value)
        {
            pointer.SetDataWord(index, unchecked((uint)*(uint*)&value), (ulong)0xFFFFFFFF);
        }
        public override FixedSizeList<float> CreateList(Pointer pointer, int count) { return (FixedSizeList<float>)pointer.Allocate(ElementSize.FourBytes, count); }
    }
    internal class DoubleStructAccessor : BasicTypeAccessor<double>
    {
        public override unsafe double Get(Pointer pointer, int index)
        {
            ulong value = pointer.GetDataWord(index);
            return *(double*)(&value);
        }
        public override unsafe void Set(Pointer pointer, int index, double value)
        {
            pointer.SetDataWord(index, *(ulong*)(&value), ~(ulong)0);
        }
        public override FixedSizeList<double> CreateList(Pointer pointer, int count) { return (FixedSizeList<double>)pointer.Allocate(ElementSize.EightBytesNonPointer, count); }
    }
    internal abstract class FailStructAccessor<T> : BasicTypeAccessor<T>
    {
        protected abstract Exception Fail();
        public override T Get(Pointer pointer, int index) { throw Fail(); }
        public override void Set(Pointer pointer, int index, T value) { throw Fail(); }
        public override FixedSizeList<T> CreateList(Pointer pointer, int count) { throw Fail(); }
        public override T Create(Pointer pointer) { throw Fail(); }
    }
    internal class MissingMetadataStructAccessor <T> : FailStructAccessor<T>
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
    // THIS BOXES; it is just a lazy stop-gap to show it working
    // TODO: replace this shit
    internal class DynamicPointerStructAccessor<T> : BasicTypeAccessor<T>
    {
        public override T Get(Pointer pointer, int index)
        {
            return (T)(dynamic)pointer.GetPointer(index);
        }

        public override void Set(Pointer pointer, int index, T value)
        {
            pointer.SetPointer(index, (Pointer)(dynamic)value);
        }
        public DynamicPointerStructAccessor(ElementSize elementSize, short dataWords, short pointers)
        {
            this.elementSize = elementSize;
            this.dataWords = dataWords;
            this.pointers = pointers;
        }
        private readonly ElementSize elementSize;
        private readonly short dataWords, pointers;

        public override FixedSizeList<T> CreateList(Pointer pointer, int count)
        {
            Pointer list;
            if(elementSize == ElementSize.InlineComposite)
            {
                list = pointer.Allocate(dataWords, pointers, count);
            }
            else
            {
                list = pointer.Allocate(elementSize, count);
            }
            return (FixedSizeList<T>)(dynamic)list;
        }
        public override T Create(Pointer pointer)
        {
            return (T)(dynamic)pointer.Allocate(dataWords, pointers);
        }
    }

    public struct FixedSizeList<T> : ICollection<T>
    {
        public override string ToString()
        {
            return pointer.ToString();
        }
        private readonly Pointer pointer;
        private FixedSizeList(Pointer pointer) { this.pointer = pointer; }
        public static explicit operator FixedSizeList<T>(Pointer pointer) { return new FixedSizeList<T>(pointer); }
        public static implicit operator Pointer(FixedSizeList<T> obj) { return obj.pointer; }
        public static bool operator true(FixedSizeList<T> obj) { return obj.pointer.IsValid; }
        public static bool operator false(FixedSizeList<T> obj) { return !obj.pointer.IsValid; }
        public static bool operator !(FixedSizeList<T> obj) { return !obj.pointer.IsValid; }
        
        static readonly StructAccessor<T> accessor = StructAccessor<T>.Instance;
        public T this[int index]
        {
            get { return accessor.Get(pointer, index); }
            set { accessor.Set(pointer, index, value); }
        }
        public int Count() { return pointer.Count(); }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            
            int count = Count();
            if(count != 0)
            {
                var comparer = EqualityComparer<T>.Default;
                var accessor = FixedSizeList<T>.accessor;
                var pointer = this.pointer;
                for (int i = 0; i < count; i++ )
                {
                    if (comparer.Equals(accessor.Get(pointer, i), item)) return true;
                }
            }
            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            int count = Count();
            if (count != 0)
            {
                var accessor = FixedSizeList<T>.accessor;
                var pointer = this.pointer;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex++] = accessor.Get(pointer, i);
                }
            }
        }

        int ICollection<T>.Count
        {
            get { return Count(); }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            int count = Count();
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static FixedSizeList<T> Create(Pointer pointer, int count)
        {
            return accessor.CreateList(pointer, count);
        }
    }
}
