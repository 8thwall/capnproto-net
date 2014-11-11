//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;

//namespace CapnProto
//{
//    abstract unsafe class CapnProtoRawMemoryReader : CapnProtoReader
//    {
//        protected override void OnChangeSegment(int index, SegmentRange range)
//        {
//            // nothing to do
//        }
        
//        private long offset, count;
//        protected long Offset { get { return offset; } }
//        protected long Count { get { return count; } }
//        protected ulong*[] segments;

//        Decoder decoder;
//        protected override string ReadString(int segment, int origin, int count)
//        {
//            byte* root = (byte*)(segments[segment] + origin);
//            if (root[count - 1] != 0) throw ExpectedNullTerminatedString();
//            var dec = decoder;
//            if (dec == null) decoder = dec = Encoding.UTF8.GetDecoder();
//            else dec.Reset();
//            int len = dec.GetCharCount(root, count - 1, true);
//            char* c = stackalloc char[len];
//            dec.GetChars(root, count - 1, c, len, false);
//            string s = new string(c, 0, len);
//            return s;
//        }

//        internal void Init(long offset, long count, object context)
//        {
//            this.offset = offset;
//            this.count = count;
//            base.Init(context);
//        }

//        protected override byte[] ReadBytes(int segment, int wordOffset, int count)
//        {
//            if (count == 0) return NilBytes;
//            var root = segments[segment] + wordOffset;
//            var arr = new byte[count];
//            Marshal.Copy(new IntPtr(root), arr, 0, count);
//            return arr;
//        }
//        protected override void Reset(bool recycling)
//        {
//            if (recycling)
//            {
//                offset = count = 0;
//                if (segments != null)
//                    Array.Clear(segments, 0, segments.Length);
//            }
//            else
//            {
//                segments = null;
//                decoder = null;
//            }
//            base.Reset(recycling);
//        }
//        public override ulong ReadWord(int segment, int wordOffset)
//        {
//            return segments[segment][wordOffset];
//        }

//        public override void ReadWords(int segment, int wordOffset, byte[] buffer, int bufferOffset, int wordCount)
//        {
//            ulong* root = segments[segment] + wordOffset;
//            Marshal.Copy(new IntPtr(root), buffer, bufferOffset, 8 * wordCount);
//        }

//        protected override unsafe void ReadWords(int segment, int origin, int available, int expected, ulong* raw)
//        {
//            int wordsToRead = Math.Max(available, expected);
//            ulong* root = segments[segment] + origin;
//            for (int i = 0; i < wordsToRead; i++)
//            {
//                raw[i] = root[i];
//            }
//        }
//    }
//}
