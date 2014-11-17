using System;
using System.Runtime.InteropServices;
#if UNSAFE
namespace CapnProto
{
    public class BufferPointerSegment : PointerSegment
    {
        GCHandle handle;
        public static BufferPointerSegment Create()
        {
            return new BufferPointerSegment();
        }
        public override void Reset(bool recycling)
        {
            if (handle.IsAllocated) handle.Free();
            handle = default(GCHandle);
            base.Reset(recycling);
        }
        ~BufferPointerSegment()
        {
            if (handle.IsAllocated) handle.Free();
        }
        public unsafe void Init(byte[] buffer, int offset, int capacityWords, int activeWords)
        {
            if (activeWords < capacityWords) Array.Clear(buffer, offset, capacityWords - activeWords);
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                var ptr = (byte*)handle.AddrOfPinnedObject();
                base.Initialize(new IntPtr(ptr + offset), capacityWords, activeWords);
                this.handle = handle;
                handle = default(GCHandle);
            }
            finally
            {
                if (handle.IsAllocated) handle.Free();
            }
        }
    }
}
#endif