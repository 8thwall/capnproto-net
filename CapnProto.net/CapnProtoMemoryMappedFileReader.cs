
//using System;
//using System.IO;
//using System.IO.MemoryMappedFiles;
//using System.Text;
//namespace CapnProto
//{
//    internal sealed class CapnProtoMemoryMappedFileReader : CapnProtoRawMemoryReader
//    {
//        private MemoryMappedFile file;

//        public new static CapnProtoMemoryMappedFileReader Create(string path, object context, long offset, long length)
//        {
//            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");

//            MemoryMappedFile newFile = null;
//            try
//            {
//                if(length < 0) length = new FileInfo(path).Length;
//                newFile = MemoryMappedFile.CreateFromFile(path, System.IO.FileMode.Open, null, 0L, MemoryMappedFileAccess.Read);
//                var obj = Cache<CapnProtoMemoryMappedFileReader>.Pop() ?? new CapnProtoMemoryMappedFileReader();
//                obj.Init(newFile, context, offset, length);
//                newFile = null;
//                return obj;
//            }
//            finally
//            {
//                if (newFile != null) newFile.Dispose();
//            }
//        }

//        private void Init(MemoryMappedFile file, object context, long offset, long length)
//        {
//            base.Init(offset, length, context);
//            this.file = file;
//        }
//        private CapnProtoMemoryMappedFileReader() { }
//        protected override void OnDispose(bool disposing)
//        {
//            if (disposing)
//            {
//                Cache<CapnProtoMemoryMappedFileReader>.Push(this);
//            }
//            else
//            {
//                base.OnDispose(false);
//            }
//        }
//        protected override void Reset(bool recycling)
//        {
//            if (file != null)
//            {
//                file.Dispose();
//                file = null;
//            }
//            if(acquired != null)
//            {
//                for(int i = 0 ; i < acquired.Length ; i++)
//                {
//                    var acc = acquired[i];
//                    if (acc != null) 
//                    {
//                        acc.SafeMemoryMappedViewHandle.ReleasePointer();
//                        acc.Dispose();
//                        acquired[i] = null;
//                    }
                    
//                }
//            }
//            if (!recycling)
//            {
//                acquired = null;
//            }
//            base.Reset(recycling);
//        }

//        MemoryMappedViewAccessor[] acquired;
//        protected unsafe override void OnInitialize()
//        {
//            var baseOffset = Offset + UnderlyingBaseOffset;
//            var count = SegmentCount;
//            var tmp = segments;
//            if (count == 0)
//            {
//                if (tmp != null) Array.Clear(tmp, 0, tmp.Length);
//            }
//            else
//            {
//                if (tmp == null || tmp.Length < count)
//                {
//                    tmp = new ulong*[count];
//                    acquired = new MemoryMappedViewAccessor[count];
//                }
//                else
//                {
//                    Array.Clear(tmp, count, tmp.Length - count);
//                    Array.Clear(acquired, count, tmp.Length - count);
//                }
//                for (int i = 0; i < count; i++)
//                {
//                    var segment = GetSegment(i);
//                    MemoryMappedViewAccessor newAcc = null;
//                    try
//                    {
//                        newAcc = file.CreateViewAccessor(baseOffset + (8 * segment.Offset), 8 * segment.Length, MemoryMappedFileAccess.Read);
//                        byte* ptr = (byte*)0;
//                        newAcc.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
//                        acquired[i] = newAcc;
//                        tmp[i] = (ulong*)ptr;
//                        newAcc = null; // to prevent immediate dispose
//                    }
//                    finally
//                    {
//                        if (newAcc != null) newAcc.Dispose(); // to clean everything up nicely if we have severe problems
//                    }
//                }
//            }
//            segments = tmp;
//            base.OnInitialize();
//        }

//        protected override bool TryReadWordDirect(long byteOffset, out ulong word)
//        {
//            if (byteOffset < 0 || byteOffset + 8 >= Count)
//            {
//                word = 0;
//                return false;
//            }
//            using(var acc = file.CreateViewAccessor(byteOffset + Offset, 8, MemoryMappedFileAccess.Read))
//            {
//                word = acc.ReadUInt64(0);
//                return true;
//            }
//        }

//    }
//}
