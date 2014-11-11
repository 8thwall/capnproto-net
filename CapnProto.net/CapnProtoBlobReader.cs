//using System;
//using System.Runtime.InteropServices;

//namespace CapnProto
//{
//    internal sealed class CapnProtoBlobReader : CapnProtoRawMemoryReader
//    {
//        GCHandle handle;
        
//        public static CapnProtoBlobReader Create(byte[] blob, int offset, int count, object context)
//        {
//            if (blob == null) throw new ArgumentNullException("blob");
//            if (offset < 0) throw new ArgumentOutOfRangeException("offset");
//            if (count < 0)
//            { // interpret as: all of it
//                count = blob.Length - offset;
//            }
//            if (offset + count > blob.Length) throw new ArgumentOutOfRangeException("count");

//            var obj = Cache<CapnProtoBlobReader>.Pop() ?? new CapnProtoBlobReader();
//            obj.Init(blob, offset, count, context);
//            return obj;
//        }
//        private void Init(byte[] blob, int offset, int count, object context)
//        {
//            handle = GCHandle.Alloc(blob, GCHandleType.Pinned);
//            base.Init(offset, count, context);
//        }
//        private CapnProtoBlobReader() { }
//        ~CapnProtoBlobReader()
//        {
//            OnDispose(false);
//        }

//        protected unsafe override bool TryReadWordDirect(long byteOffset, out ulong word)
//        {
//            if (byteOffset < 0 || byteOffset + 8 >= Count)
//            {
//                word = 0;
//                return false;
//            }
//            byte* ptr = ((byte*)handle.AddrOfPinnedObject()) + (int)byteOffset + Offset;
//            word = ((ulong*)ptr)[0];
//            return true;
//        }
        

//        protected override void Reset(bool recycling)
//        {
//            if (handle.IsAllocated)
//            {
//                handle.Free();
//                handle = default(GCHandle);
//            }
//            base.Reset(recycling);
//        }
//        protected override void OnDispose(bool disposing)
//        {
//            if(disposing)
//            {
//                Cache<CapnProtoBlobReader>.Push(this);
//            }
//            else
//            {
//                if (handle.IsAllocated)
//                {
//                    handle.Free();
//                    handle = default(GCHandle);
//                }
//                base.OnDispose(false);
//            }
//        }

//        protected unsafe override void OnInitialize()
//        {
//            byte* basePtr = (byte*)handle.AddrOfPinnedObject();
//            basePtr = basePtr + Offset + (int)UnderlyingBaseOffset;
//            var count = SegmentCount;

//            var tmp = segments;
//            if(count == 0)
//            {
//                if (tmp != null) Array.Clear(tmp, 0, tmp.Length);
//            }
//            else
//            {
//                if (tmp == null || tmp.Length < count) tmp = new ulong*[count];
//                else Array.Clear(tmp, count, tmp.Length - count);
//                for (int i = 0; i < count; i++)
//                {
//                    tmp[i] = (ulong*)(basePtr + (8 * GetSegment(i).Offset));
//                }
//            }
//            segments = tmp;
//            base.OnInitialize();
//        }
//    }
//}
