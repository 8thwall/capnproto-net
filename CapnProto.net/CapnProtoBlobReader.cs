using System;
using System.Runtime.InteropServices;

namespace CapnProto
{
    internal unsafe class CapnProtoBlobReader : CapnProtoReader
    {
        GCHandle handle;
        private int offset, count;
        ulong*[] segments;

        [ThreadStatic]
        private static CapnProtoBlobReader recycled;

        public static CapnProtoBlobReader Create(byte[] blob, int offset, int count, object context)
        {
            if (blob == null) throw new ArgumentNullException("blob");
            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
            { // interpret as: all of it
                count = blob.Length - offset;
            }
            if (offset + count > blob.Length) throw new ArgumentOutOfRangeException("count");

            var tmp = recycled;
            if(tmp != null)
            {
                recycled = null;
                GC.ReRegisterForFinalize(tmp);
                tmp.Init(blob, offset, count, context);
                return tmp;
            }
            return new CapnProtoBlobReader(blob, offset, count, context);
        }
        private void Init(byte[] blob, int offset, int count, object context)
        {
            this.offset = offset;
            this.count = count;
            handle = GCHandle.Alloc(blob, GCHandleType.Pinned);
            base.Init(context);
        }
        private CapnProtoBlobReader(byte[] blob, int offset, int count, object context)
        {
            Init(blob, offset, count, context);
        }
        ~CapnProtoBlobReader()
        {
            OnDispose(false);
        }

        protected override bool TryReadWordDirect(long byteOffset, out ulong word)
        {
            if (byteOffset < 0 || byteOffset + 8 >= count)
            {
                word = 0;
                return false;
            }
            byte* ptr = ((byte*)handle.AddrOfPinnedObject()) + (int)byteOffset;
            word = ((ulong*)ptr)[0];
            return true;
        }
        protected override byte[] ReadBytes(int segment, int wordOffset, int count)
        {
            var root = segments[segment] + wordOffset;
            var arr = new byte[count];
            Marshal.Copy(new IntPtr(root), arr, 0, count);
            return arr;
        }

        protected override void Reset(bool recycling)
        {
            base.Reset(recycling);
            if (handle.IsAllocated)
            {
                handle.Free();
                handle = default(GCHandle);
            }
            if (recycling)
            {
                offset = count = 0;
                if(segments != null)
                    Array.Clear(segments, 0, segments.Length);
            }
            else
            {
                segments = null;
            }
        }
        protected override void OnDispose(bool disposing)
        {
            if(disposing)
            {
                if(recycled == null)
                {
                    // note: don't want to add GC.SuppressFinalize
                    // to Reset, in case Reset is called independently
                    // of lifetime management
                    Reset(true);
                    GC.SuppressFinalize(this);
                    recycled = this;
                }
                else
                {
                    Reset(false);
                    GC.SuppressFinalize(this);
                }
            }
            else
            {
                if (handle.IsAllocated)
                {
                    handle.Free();
                    handle = default(GCHandle);
                }
            }
        }
        public override ulong ReadWord(int segment, int wordOffset)
        {
            return segments[segment][wordOffset];
        }

        public override void ReadWords(int segment, int wordOffset, byte[] buffer, int bufferOffset, int wordCount)
        {
            ulong* root = segments[segment] + wordOffset;
            Marshal.Copy(new IntPtr(root), buffer, bufferOffset, 8 * wordCount);
        }

        protected override void OnChangeSegment(int index, SegmentRange range)
        {
            // nothing to do
        }
        protected override unsafe void ReadWords(int segment, int origin, int available, int expected, ulong* raw)
        {
            int wordsToRead = Math.Max(available, expected);
            ulong* root = segments[segment] + origin;
            for (int i = 0; i < wordsToRead; i++)
            {
                raw[i] = root[i];
            } 
        }
        protected override void OnInitialize()
        {
            byte* basePtr = (byte*)handle.AddrOfPinnedObject();
            basePtr = basePtr + offset + (int)UnderlyingBaseOffset;
            var count = SegmentCount;

            var tmp = segments;
            if(count == 0)
            {
                if (tmp != null) Array.Clear(tmp, 0, tmp.Length);
                System.Diagnostics.Debugger.Break();
            }
            else
            {
                if (tmp == null || tmp.Length < count) tmp = new ulong*[count];
                else Array.Clear(tmp, count, tmp.Length - count);
                for (int i = 0; i < count; i++)
                {
                    tmp[i] = (ulong*)(basePtr + (8 * GetSegment(i).Offset));
                }
            }
            segments = tmp;
            base.OnInitialize();
        }
    }
}
