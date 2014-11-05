
using System;
using System.IO;
using System.Text;
namespace CapnProto
{
    internal class CapnProtoStreamReader : CapnProtoReader
    {
        private System.IO.Stream source;
        private readonly bool leaveOpen;

        protected override bool TryReadWordDirect(long byteOffset, out ulong word)
        {
            var buffer = Scratch;
            int count = 8, read, offset = 0;
            source.Position = byteOffset;
            while((read = source.Read(buffer, offset, count)) > 0)
            {
                count -= read;
                offset += read;
            }
            if (count == 0)
            {
                word = BitConverter.ToUInt64(buffer, 0);
                return true;
            }
            else
            {
                word = 0;
                return false;
            }
        }
        public CapnProtoStreamReader(Stream source, object context, bool leaveOpen) : base(context)
        {
            if (source == null) throw new ArgumentNullException("source");
            this.leaveOpen = leaveOpen;
            this.source = source;
        }

        static CapnProtoStreamReader()
        {
            TypeModel.AssertLittleEndian();
        }

        public override void Dispose()
        {
            if (source != null && !leaveOpen) source.Dispose();
            source = null;
        }


        protected override void OnChangeSegment(int segment, SegmentRange range)
        {
            segmentStart = UnderlyingBaseOffset + (8 * range.Offset);
        }

        protected override string ReadString(int segment, int wordOffset, int count)
        {
            // this is a terrible implementation
            var bytes = ReadBytes(segment, wordOffset, count);
            if (bytes[count - 1] != 0) throw ExpectedNullTerminatedString();
            return Encoding.UTF8.GetString(bytes, 0, count - 1);
        }
        protected override byte[] ReadBytes(int segment, int wordOffset, int count)
        {
            byte[] arr = new byte[count];
            CopyBytes(segment, wordOffset, arr, 0, count);
            return arr;
        }

        private void CopyBytes(int segment, int wordOffset, byte[] buffer, int bufferOffset, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException("count");
            if (Segment != segment)
            {
                ChangeSegment(segment);
            }
            source.Position = checked(UnderlyingBaseOffset + (wordOffset * 8));
            int read;

            if ((read = source.Read(buffer, bufferOffset, count)) == count) return; // got it all first try; nice

            if (read > 0)
            {  // got *something*; keep trying
                count -= read;
                bufferOffset += read;
                while (count != 0 && (read = source.Read(buffer, bufferOffset, count)) > 0)
                {
                    count -= read;
                    bufferOffset += read;
                }
            }
            if (count != 0) throw new EndOfStreamException();
        }

        public override void ReadWords(int segment, int wordOffset, byte[] buffer, int bufferOffset, int wordCount)
        {
            CopyBytes(segment, wordOffset, buffer, bufferOffset, checked(wordCount * 8));
        }

        long segmentStart;
    }
}
