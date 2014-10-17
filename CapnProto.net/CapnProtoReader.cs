using System;
using System.IO;
using System.Text;

namespace CapnProto
{
    abstract public partial class CapnProtoReader : IDisposable
    {
        protected const int ScratchLength = 64; // must be at least 8
        protected readonly byte[] Scratch = new byte[ScratchLength];
        public static CapnProtoReader Create(Stream source, object context = null, bool leaveOpen = false)
        {
            return new CapnProtoStreamReader(source, context, leaveOpen);
        }

        protected CapnProtoReader(object context)
        {
            this.context = context;
            
        }

        private static readonly bool isLittleEndian = BitConverter.IsLittleEndian;
        
        public virtual void Dispose() { context = null; }

        private object context;
        public object Context { get { return context; } }

        //private void Read(byte[] buffer, int offset, int count)
        //{
        //    int read;
        //    while(count > 0 && (read = ReadRaw(buffer, offset, count)) > 0)
        //    {
        //        count -= read;
        //        offset += read;
        //    }
        //    if (count != 0) throw new EndOfStreamException();
        //}



        public abstract void ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int wordCount);
        public virtual ulong ReadWord(int wordOffset)
        {
            var tmp = Scratch;
            ReadWords(wordOffset, tmp, 0, 1);
            return BitConverter.ToUInt64(tmp, 0);
        }

        protected abstract void OnChangeSegment(int segment, int offset, int length);

        private int segment;
        public int Segment { get { return segment; } }
        public void ChangeSegment(int segment)
        {
            if (segment != this.segment)
            {
                int offset, length;
                if (segment == 0)
                {
                    offset = firstSegment.Offset;
                    length = firstSegment.Length;
                }
                else
                {

                    if (segment < 0 || segment-- > otherSegments.Length)
                        throw new ArgumentOutOfRangeException("segment");

                    offset = otherSegments[segment].Offset;
                    length = otherSegments[segment].Length;
                }
                OnChangeSegment(segment, offset, length);
                this.segment = segment;
            }
        }
        public byte[] ReadBytesFromPointer(int wordOffset, ulong ptr)
        {
            var lptr = new ListPointer(ptr);
            if (lptr.ElementSize != ElementSize.OneByte) throw new InvalidOperationException("Expected pointer to single-byte data");

            int len = lptr.Size;
            if (len == 0) return NixBytes;
            return ReadBytes(wordOffset + lptr.Offset, len);
        }
        static readonly byte[] NixBytes = new byte[0];
        public string ReadStringFromPointer(int wordOffset, ulong ptr)
        {
            var lptr = new ListPointer(ptr);
            if (lptr.ElementSize != ElementSize.OneByte) throw new InvalidOperationException("Expected pointer to single-byte data");

            int len = lptr.Size;
            if (len == 0) throw ExpectedNullTerminatedString();

            if (len <= ScratchLength)
            {
                var tmp = Scratch;
                ReadWords(wordOffset + lptr.Offset, tmp, 0, ToWords(len));
                if (tmp[len - 1] != 0) throw ExpectedNullTerminatedString();

                return Encoding.UTF8.GetString(tmp, 0, len - 1);

            }
            return ReadString(wordOffset + lptr.Offset, len);
        }
        private static int ToWords(int bytes)
        {
            return (bytes + 7) / 8;
        }
        protected Exception ExpectedNullTerminatedString()
        {
            return new InvalidOperationException("Expected nul-terminated string");
        }

        protected abstract string ReadString(int wordOffset, int count);
        protected abstract byte[] ReadBytes(int wordOffset, int count);

        private SegmentRange firstSegment;
        private SegmentRange[] otherSegments;
        public virtual void ReadPreamble()
        {
            int offset = 0;
            var word = ReadWord(offset++);
            int additionalSegments = (int)word;

            int segmentOffset = 1 + (additionalSegments / 2);
            if ((additionalSegments % 2) != 0) segmentOffset++;
            firstSegment = new SegmentRange(ref segmentOffset, (int)(word >> 32));
            if (additionalSegments != 0)
            {
                otherSegments = new SegmentRange[additionalSegments];
                int index = 0;
                for (int i = 0; i < additionalSegments / 2; i++)
                {
                    word = ReadWord(offset++);
                    otherSegments[index++] = new SegmentRange(ref segmentOffset, (int)word);
                    otherSegments[index++] = new SegmentRange(ref segmentOffset, (int)(word >> 32));
                }

                if ((additionalSegments % 2) != 0)
                {
                    word = ReadWord(offset++);
                    otherSegments[index] = new SegmentRange(ref segmentOffset, (int)word);
                }
            }
            OnChangeSegment(0, firstSegment.Offset, firstSegment.Length);
        }
        private struct SegmentRange
        {
            public readonly int Offset, Length;
            public SegmentRange(ref int offset, int length)
            {
                this.Offset = offset;
                this.Length = length;
                offset += length;
            }
        }

    }
}
