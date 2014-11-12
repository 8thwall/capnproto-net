
using System;
using System.Collections;
using System.Collections.Generic;
namespace CapnProto
{
    public struct FixedSizeList<T> : IList<T>, IList, IPointer
    {
        public static explicit operator FixedSizeList<T>(Pointer pointer) { return new FixedSizeList<T>(pointer); }
        public static implicit operator Pointer(FixedSizeList<T> obj) { return obj.pointer; }
        private readonly Pointer pointer;
        private FixedSizeList(Pointer pointer) { this.pointer = pointer; }
        public override bool Equals(object obj)
        {
            return obj is FixedSizeList<T> && ((FixedSizeList<T>)obj).pointer == this.pointer;
        }
        public override int GetHashCode() { return this.pointer.GetHashCode(); }
        public override string ToString() { return pointer.ToString(); }

        public T this[int index]
        {
            get { return StructAccessor<T>.Instance.Get(pointer, index); }
            set { StructAccessor<T>.Instance.Set(pointer, index, value); }
        }
        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        void IList<T>.Insert(int index, T value) { throw new NotSupportedException(); }
        void IList.Insert(int index, object value) { throw new NotSupportedException(); }
        void IList<T>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList.Remove(object value) { throw new NotSupportedException(); }
        void IList.RemoveAt(int index) { throw new NotSupportedException(); }
        public int Count() { return pointer.Count(); }

        void ICollection<T>.Add(T item) { throw new NotSupportedException(); }
        int IList.Add(object value) { throw new NotSupportedException(); }
        void ICollection<T>.Clear() { throw new NotSupportedException(); }
        void IList.Clear() { throw new NotSupportedException(); }

        public bool Contains(T value)
        {

            var pointer = this.pointer.Dereference();
            int count = pointer.Count();
            if (count != 0)
            {
                var comparer = EqualityComparer<T>.Default;
                var accessor = StructAccessor<T>.Instance;
                for (int i = 0; i < count; i++)
                {
                    if (comparer.Equals(accessor.Get(pointer, i), value)) return true;
                }
            }
            return false;
        }
        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }
        public int IndexOf(T value)
        {
            var pointer = this.pointer.Dereference();
            int count = pointer.Count();
            if (count != 0)
            {
                var comparer = EqualityComparer<T>.Default;
                var accessor = StructAccessor<T>.Instance;
                for (int i = 0; i < count; i++)
                {
                    if (comparer.Equals(accessor.Get(pointer, i), value)) return i;
                }
            }
            return -1;
        }
        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            var pointer = this.pointer.Dereference();
            int count = pointer.Count();
            if (count != 0)
            {
                var accessor = StructAccessor<T>.Instance;
                for (int i = 0; i < count; i++)
                {
                    array[arrayIndex++] = accessor.Get(pointer, i);
                }
            }
        }
        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            var pointer = this.pointer.Dereference();
            int count = pointer.Count();
            if (count != 0)
            {
                var accessor = StructAccessor<T>.Instance;
                for (int i = 0; i < count; i++)
                {
                    array.SetValue(accessor.Get(pointer, i), arrayIndex++);
                }
            }
        }

        int ICollection<T>.Count { get { return Count(); } }
        int ICollection.Count { get { return Count(); } }

        bool ICollection.IsSynchronized { get { return false; } }
        object ICollection.SyncRoot { get { return null; } }

        bool ICollection<T>.IsReadOnly { get { return false; } }
        bool IList.IsReadOnly { get { return false; } }
        bool IList.IsFixedSize { get { return true; } }

        bool ICollection<T>.Remove(T item) { throw new NotSupportedException(); }

        public IEnumerator<T> GetEnumerator()
        {
            var pointer = this.pointer.Dereference();
            int count = pointer.Count();
            if (count != 0)
            {
                var accessor = StructAccessor<T>.Instance;
                for (int i = 0; i < count; i++)
                    yield return accessor.Get(pointer, i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static FixedSizeList<T> Create(Pointer pointer, int count)
        {
            return StructAccessor<T>.Instance.CreateList(pointer, count);
        }

        public static FixedSizeList<T> Create(Pointer pointer, IList<T> items)
        {
            if (items == null) return default(FixedSizeList<T>);
            if (items.Count == 0) return FixedSizeList<T>.Create(pointer, 0);

            var accessor = StructAccessor<T>.Instance;
            FixedSizeList<T> list = accessor.IsPointer
                ? accessor.CreateList(pointer, items.Count, ElementSize.EightBytesPointer)
                : accessor.CreateList(pointer, items.Count);
            for (int i = 0; i < items.Count; i++)
            {
                list[i] = items[i];
            }
            return list;
        }
        Pointer IPointer.Pointer { get { return pointer; } }
    }
}
