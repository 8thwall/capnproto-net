
//using System;
//using System.IO;
//using System.Text;
//namespace CapnProto
//{
//    internal sealed class CapnProtoStreamReader : CapnProtoReader
//    {
//        private System.IO.Stream source;
//        private bool leaveOpen;

//        public override string ToString()
//        {
//            return GetType().Name + " / " + (source == null ? "nil" : source.GetType().Name);
//        }
//        protected override bool TryReadWordDirect(long byteOffset, out ulong word)
//        {
//            var buffer = Scratch;
//            int count = 8, read, offset = 0;
//            source.Position = byteOffset;
//            while((read = source.Read(buffer, offset, count)) > 0)
//            {
//                count -= read;
//                offset += read;
//            }
//            if (count == 0)
//            {
//                word = BitConverter.ToUInt64(buffer, 0);
//                return true;
//            }
//            else
//            {
//                word = 0;
//                return false;
//            }
//        }

//        public static new CapnProtoStreamReader Create(Stream source, object context, bool leaveOpen)
//        {
//            if (source == null) throw new ArgumentNullException("source");

//            var obj = Cache<CapnProtoStreamReader>.Pop() ?? new CapnProtoStreamReader();
//            obj.Init(source, context, leaveOpen);
//            return obj;
//        }
//        private CapnProtoStreamReader() { }
//        protected override void Reset(bool recycling)
//        {
//            if(source != null)
//            {
//                if (!leaveOpen) source.Dispose();
//                source = null;
//            }
//            if(recycling)
//            {
//                segmentStart = 0;
//            }
//            base.Reset(recycling);
//        }
//        private void Init(Stream source, object context, bool leaveOpen)
//        {
//            this.leaveOpen = leaveOpen;
//            this.source = source;
//            base.Init(context);
//        }

//        static CapnProtoStreamReader()
//        {
//            TypeModel.AssertLittleEndian();
//        }

//        protected override void OnDispose(bool disposing)
//        {
//            if (disposing)
//            {
//                Cache<CapnProtoStreamReader>.Push(this);
//            }
//            else
//            {
//                base.OnDispose(false);
//            }
//        }


//        protected override void OnChangeSegment(int segment, SegmentRange range)
//        {
//            segmentStart = UnderlyingBaseOffset + (8 * range.Offset);
//        }

//        protected override byte[] ReadBytes(int segment, int wordOffset, int count)
//        {
//            byte[] arr = new byte[count];
//            CopyBytes(segment, wordOffset, arr, 0, count);
//            return arr;
//        }

//        private void CopyBytes(int segment, int wordOffset, byte[] buffer, int bufferOffset, int count)
//        {
//            if (count <= 0) throw new ArgumentOutOfRangeException("count");
//            if (Segment != segment)
//            {
//                ChangeSegment(segment);
//            }
            
//            source.Position = checked(segmentStart + (wordOffset * 8));
//            int read;

//            if ((read = source.Read(buffer, bufferOffset, count)) == count) return; // got it all first try; nice

//            if (read > 0)
//            {  // got *something*; keep trying
//                count -= read;
//                bufferOffset += read;
//                while (count != 0 && (read = source.Read(buffer, bufferOffset, count)) > 0)
//                {
//                    count -= read;
//                    bufferOffset += read;
//                }
//            }
//            if (count != 0) throw new EndOfStreamException();
//        }

//        public override void ReadWords(int segment, int wordOffset, byte[] buffer, int bufferOffset, int wordCount)
//        {
//            CopyBytes(segment, wordOffset, buffer, bufferOffset, checked(wordCount * 8));
//        }

//        long segmentStart;
//    }
//}
