
using System;
using System.IO;
namespace CapnProto.Take2
{
    public struct Pointer
    {
        internal Pointer(ISegment segment, int headerIndex, int listIndex)
        {
            this.segment = segment;
            this.header = segment[headerIndex];
            this.listIndex = listIndex;
            if ((header & 2) == 0)
            {
                // object header; offset is (signed) bits 3-31
                this.start = headerIndex + 1 + (unchecked((int)(uint)header) >> 2);
            }
            else
            {
                this.start = int.MinValue;
            }            
        }
        private Pointer(ISegment segment, ulong header, int start, int listIndex)
        {
            this.segment = segment;
            this.header = header;
            this.start = start;
            this.listIndex = listIndex;
        }
        private readonly ISegment segment;
        private readonly ulong header;
        private readonly int start, listIndex;
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
        private Pointer Dereference()
        {
            
            var segment = this.segment;            
            if (segment == null) throw new NullReferenceException("Cannot dereference a nil pointer");
            var header = this.header;
            int wordIndex;
            switch (header & 3)
            {
                case 0: // struct-pointer
                case 1: // list-pointer
                    return this;
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
            return new Pointer(segment, header, wordIndex + offset, listIndex);
        }

        private ulong GetDataWord(int index)
        {
            if (index < 0) return 0;
            if ((header & 2) != 0) return Dereference().GetDataWord(index);

            uint rhs = unchecked((uint)(header >> 32));
            int count = unchecked((int)(rhs & 0xFFFF));
            return index < count ? segment[start + index] : 0;
        }
        public Pointer GetPointer(int index)
        {
            if (index < 0) return default(Pointer);
            if ((header & 2) != 0) return Dereference().GetPointer(index);

            uint rhs = unchecked((uint)(header >> 32));
            int count = unchecked((int)(rhs >> 16));
            if (index >= count) return default(Pointer);
            return new Pointer(segment, start + unchecked((int)(rhs & 0xFFFF)) + index, 0);
        }

        public bool IsValid { get { return segment != null; } }

        public Pointer Allocate(int dataWords, int pointers)
        {
            ulong header = (checked((ulong)dataWords) << 32) | (checked((ulong)pointers) << 48);
            int start, size = dataWords + pointers;
            if (segment.TryAllocate(size, out start))
            { // in-segment pointer
                return new Pointer(segment, header, start, 0);
            }
            else
            {
                // far pointer
                int segIndex = segment.Index + 1;
                var msg = segment.Message;
                start = msg.Allocate(ref segIndex, size + 1);
                msg[segIndex][start] = header;
                uint lhs = unchecked((uint)((start << 3) | 2)), rhs = unchecked((uint)segIndex);
                header = (ulong)lhs | (((ulong)rhs) << 32);
                return new Pointer(segment, header, 0, 0);
            }
            throw new NotImplementedException();
            
            
        }
    }
}
