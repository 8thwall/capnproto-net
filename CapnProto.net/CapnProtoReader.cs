using System;
using System.IO;

namespace CapnProto
{
    abstract public partial class CapnProtoReader : IDisposable
    {
        private byte[] scratch;
        protected byte[] Scratch { get { return scratch ?? (scratch = new byte[8]); } }
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

        private void Read(byte[] buffer, int offset, int count)
        {
            int read;
            while(count > 0 && (read = ReadRaw(buffer, offset, count)) > 0)
            {
                count -= read;
                offset += read;
            }
            if (count != 0) throw new EndOfStreamException();
        }

        public virtual StructPointer ReadStructPointer()
        {
            var raw = ReadUInt64();
            int offset = (int)raw;
            ushort dlen = (ushort)((raw >> 32) & 0xFFFF),
                plen = (ushort)((raw >> 48) & 0xFFFF);
            if ((offset & 3) != 0) throw new InvalidOperationException("Expected struct pointer");
            return new StructPointer(offset >> 2, dlen, plen);
        }

        public ListPointer ReadListPointer()
        {
            var raw = ReadUInt64();
            int offset = (int)raw,
                combinedSize = (int)(raw >> 32);
            if ((offset & 3) != 1) throw new InvalidOperationException("Expected list pointer");
            return new ListPointer(offset >> 2, combinedSize);
        }
        public abstract void SeekWords(int offset);

        public abstract int ReadRaw(byte[] buffer, int offset, int count);


        public ulong ReadUInt64()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 8);
            return isLittleEndian ? BitConverter.ToUInt64(buffer, 0)
                : (ulong)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24)
                | (buffer[4] << 32) | (buffer[5] << 40) | (buffer[6] << 48) | (buffer[7] << 56));
        }
        public long ReadInt64()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 8);
            return isLittleEndian ? BitConverter.ToInt64(buffer, 0)
                : (long)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24)
                | (buffer[4] << 32)| (buffer[5] << 40) | (buffer[6] << 48) | (buffer[7] << 56));
        }
        public uint ReadUInt32()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 4);
            return isLittleEndian ? BitConverter.ToUInt32(buffer, 0)
                : (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
        }
        public int ReadInt32()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 4);
            return isLittleEndian ? BitConverter.ToInt32(buffer, 0)
                : (int)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
        }

        public ushort ReadUInt16()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 2);
            return isLittleEndian ? BitConverter.ToUInt16(buffer, 0)
                : (ushort)(buffer[0] | (buffer[1] << 8));
        }
        public short ReadInt16()
        {
            var buffer = Scratch;
            ReadRaw(buffer, 0, 2);
            return isLittleEndian ? BitConverter.ToInt16(buffer, 0)
                : (short)(buffer[0] | (buffer[1] << 8));
        }

        public abstract void ReadPreamble();
    }
}
