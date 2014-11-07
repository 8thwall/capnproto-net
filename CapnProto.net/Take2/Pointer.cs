
using System;
using System.IO;
namespace CapnProto.Take2
{
    public struct Pointer
    {
        internal Pointer(ISegment segment, int wordIndex, int aux)
        {
            this.segment = segment;
            this.wordIndex = wordIndex;
            this.aux = aux;
        }
        private readonly ISegment segment;
        private readonly int wordIndex;
        private readonly int aux;
        // first 2 LSB types are meaning; 01 for list-index; remaining bits are value

        public bool GetBoolean(int index) {
            ulong word = GetDataWord(index / 8);
            int shift = index % 8;
            return (word >> shift) != 0;
        }
        public byte GetByte(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 3; // 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((byte)(word >> shift));
        }
        public sbyte GetSByte(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 3; // 0 => 0, 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((sbyte)(word >> shift));
        }
        public short GetInt16(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((short)(word >> shift));
        }
        public ushort GetUInt16(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((ushort)(word >> shift));
        }
        public int GetInt32(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 5; // 0 => 0, 1 => 32
            return unchecked((int)(word >> shift));
        }
        public uint GetUInt32(int index)
        {
            ulong word = GetDataWord(index / 8);
            int shift = (index % 8) << 5; // 0 => 0, 1 => 32
            return unchecked((uint)(word >> shift));
        }
        public long GetInt64(int index)
        {
            ulong word = GetDataWord(index);
            return unchecked((long)word);
        }
        public ulong GetUInt64(int index)
        {
            return GetDataWord(index);
        }
        public unsafe float GetSingle(int index)
        {
            uint val = GetUInt32(index);
            return *(float*)(&val);
        }
        public unsafe double GetDouble(int index)
        {
            ulong val = GetDataWord(index);
            return *(double*)(&val);
        }
        private void Dereference(out Payload payload)
        {
            var segment = this.segment;
            int wordIndex = this.wordIndex;
            if (segment == null) throw new NullReferenceException("Cannot dereference a nil pointer");

            var header = segment[wordIndex++];
            switch (header & 3)
            {
                case 0: // struct-pointer
                case 1: // list-pointer
                    // signed, hence want to preserve the MSB in the nibble
                    break;
                case 2: // far-pointer
                    bool isFar = (header & 4) != 0;
                    // unsigned
                    wordIndex = unchecked((int)(((uint)header) >> 3));
                    segment = segment.Message[checked((int)(header >> 32))];
                    header = segment[wordIndex++];
                    if (isFar)
                    {
                        if ((header & 3) != 2)
                            throw new InvalidDataException("Far-pointer failed to resolve to second far-pointer");
                        ulong tag = segment[wordIndex];
                        if (((uint)tag & ~3) != 0)
                            throw new InvalidDataException("Far-pointer expected zero offset");
                        wordIndex = unchecked((int)(((uint)header) >> 3));
                        segment = segment.Message[checked((int)(header >> 32))];
                        header = tag;
                    }
                    switch (header & 3)
                    {
                        case 0:
                        case 1:
                            break;
                        case 2:
                            throw new InvalidDataException("Far-pointer resolved to unexpected far-pointer");
                        case 3:
                        default:
                            throw new InvalidDataException("Far-pointer resolved to a capability pointer");
                    }
                    break;
                case 3: // capability-pointer
                default: // just to make the compiler happy
                    throw new InvalidOperationException("Cannot deference a capability pointer");
            }
            int offset = unchecked(((int)header & ~3) >> 2);
            payload.Segment = segment;
            payload.DataOffset = wordIndex + offset;
            payload.Header = header;
        }
        private struct Payload
        {
            public ISegment Segment;
            public int DataOffset;
            public ulong Header;

        }

        private ulong GetDataWord(int index)
        {
            if (index < 0) return 0;
            Payload ptr;
            Dereference(out ptr);
            uint rhs = unchecked((uint)(ptr.Header >> 32));
            int count = unchecked((int)(rhs & 0xFFFF));
            return index < count ? ptr.Segment[ptr.DataOffset + index] : 0;
        }
        public Pointer GetPointer(int index)
        {
            if (index < 0) return default(Pointer);
            Payload ptr;
            Dereference(out ptr);
            uint rhs = unchecked((uint)(ptr.Header >> 32));
            int count = unchecked((int)(rhs >> 16));
            if (index >= count) return default(Pointer);
            return new Pointer(ptr.Segment, ptr.DataOffset + unchecked((int)(rhs & 0xFFFF)) + index, 0);
        }

        public bool IsValid { get { return segment != null; } }

        public Pointer Allocate(/*uint headerIndex, */ uint dataWords, uint pointers)
        {
            //uint offset;
            //ulong header = (((ulong)dataWords) << 32) | (((ulong)pointers) << 48);

            //if (segment.TryAllocate(dataWords + pointers, out offset))
            //{ // in-segment pointer
            //    int delta = (int)offset - (int)(headerIndex + 1);
            //    header |= unchecked((uint)(delta << 2));
            //}
            //else
            //{
            //    // far pointer
            //    ulong location = segment.Message.Allocate(segment.Index + 1, dataWords + pointers + 1);
            //}
            throw new NotImplementedException();
            
            
        }
    }
}
