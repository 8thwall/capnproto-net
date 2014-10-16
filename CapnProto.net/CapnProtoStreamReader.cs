
using System;
using System.IO;
namespace CapnProto
{
    partial class CapnProtoReader
    {
        internal class CapnProtoStreamReader : CapnProtoReader
        {
            private System.IO.Stream source;
            private readonly bool leaveOpen;
            public CapnProtoStreamReader(Stream source, object context, bool leaveOpen) : base(context)
            {
                if (source == null) throw new ArgumentNullException("source");
                this.leaveOpen = leaveOpen;
                this.source = source;
            }

            public override void Dispose()
            {
                if (source != null && !leaveOpen) source.Dispose();
                source = null;
            }

            public override int ReadRaw(byte[] buffer, int offset, int count)
            {
                return source.Read(buffer, offset, count);
            }
            public override void SeekWords(int offset)
            {
                if(offset != 0) source.Seek(offset * 8, SeekOrigin.Current);
            }

            public override void ReadPreamble()
            {
                int segments = ReadInt32() + 1;
                if (segments != 1)
                    throw new NotSupportedException("Multi-segment messages are not yet supported");

                int words = ReadInt32();
            }
        }
    }
}
