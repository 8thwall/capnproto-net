using System;

namespace CapnProto
{
    public interface IRecyclable
    {
        void Reset(bool recycling);
    }
    internal static class Cache<T> where T : class, IRecyclable
    {
        [ThreadStatic]
        private static T recycled;

        public static T Pop()
        {
            var tmp = recycled;
            if (tmp != null)
            {
                recycled = null;
                GC.ReRegisterForFinalize(tmp);
                return tmp;
            }
            return null;
        }

        public static void Push(T obj)
        {
            if (obj != null)
            {
                // note: don't want to add GC.SuppressFinalize
                // to Reset, in case Reset is called independently
                // of lifetime management
                if (recycled == null)
                {
                    obj.Reset(true);
                    GC.SuppressFinalize(obj);
                    recycled = obj;
                }
                else
                {
                    obj.Reset(false);
                    GC.SuppressFinalize(obj);
                }
            }
        }
    }
}
