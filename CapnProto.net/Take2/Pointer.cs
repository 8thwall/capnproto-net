
using System;
using System.Runtime.CompilerServices;
namespace CapnProto.Take2
{
    public struct Pointer : IEquatable<Pointer>, IComparable<Pointer>
    {
        public override int GetHashCode()
        {
            ISegment seg = segment;
            unchecked
            {
                return Combine(Combine((int)startAndType, (int)dataWordsAndPointers), Combine((int)aux, seg == null ? 0 : seg.Index));
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Combine(int hash1, int hash2)
        {
            unchecked
            {
                return (((hash1 << 5) + hash1) ^ hash2);
            }
        }
        public override bool Equals(object obj)
        {
            return obj is Pointer && AreEqual(this, (Pointer)obj);
        }
        public bool Equals(Pointer other)
        {
            return AreEqual(this, other);
        }
        public int CompareTo(Pointer other)
        {
            return Compare(this, other);
        }
        public static bool operator ==(Pointer x, Pointer y) { return AreEqual(x, y); }
        public static bool operator !=(Pointer x, Pointer y) { return !AreEqual(x, y); }
        public static bool operator <(Pointer x, Pointer y) { return Compare(x, y) < 0; }
        public static bool operator >(Pointer x, Pointer y) { return Compare(x, y) > 0; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool AreEqual(Pointer x, Pointer y)
        {
            if (x.startAndType == y.startAndType && x.dataWordsAndPointers == y.dataWordsAndPointers && x.aux == y.aux)
            {
                ISegment xSeg = x.segment, ySeg = y.segment;
                // null case is obvious; same segment object case is obvious; however, "same segment" is defined as message+index,
                // not just same object instance (see: "dummy segment zero" for an example)
                return xSeg == ySeg || (xSeg != null && ySeg != null && xSeg.Index == ySeg.Index && xSeg.Message == ySeg.Message);

            }
            return false;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Compare(Pointer x, Pointer y)
        {
            unchecked
            {
                ISegment seg;
                int delta = ((seg = x.segment) == null ? -1 : seg.Index) - ((seg = y.segment) == null ? -1 : seg.Index);
                if (delta == 0)
                {
                    delta = (int)(x.startAndType >> 3) - (int)(y.startAndType >> 3);
                    if (delta == 0)
                    {
                        delta = ((int)(x.startAndType & 7)) - ((int)(y.startAndType & 7));
                        if (delta == 0)
                        {
                            delta = (int)x.dataWordsAndPointers - (int)y.dataWordsAndPointers;
                            if (delta == 0) delta = (int)x.aux - (int)y.aux;
                        }
                    }
                }
                return delta;
            }
        }
        internal Pointer(ISegment segment, int headerIndex)
        {
            unchecked
            {
                var header = segment[headerIndex];
                if (header == 0)
                {
                    // normally, this is considered a nil-pointer; we will special-case the root
                    // of a message, though:
                    if (segment.Index == 0)
                    {
                        this.segment = segment;
                        this.startAndType = 0;
                        this.dataWordsAndPointers = 0;
                        this.aux = 0;
                    }
                    else
                    {
                        this = default(Pointer);
                    }
                    return;
                }
                this.segment = segment;
                uint lhs = (uint)header, rhs = (uint)(header >> 32);
                int start;
                switch (lhs & 3)
                {
                    case Type.StructBasic:
                        start = headerIndex + 1 + (((int)lhs) >> 2);
                        startAndType = (uint)start << 3;
                        dataWordsAndPointers = rhs;
                        aux = 0;
                        break;
                    case Type.ListBasic:
                        start = headerIndex + 1 + (((int)lhs) >> 2);
                        switch ((ElementSize)(rhs & 7))
                        {
                            case ElementSize.EightBytesPointer:
                                dataWordsAndPointers = 1 << 16;
                                startAndType = ((uint)start << 3) | 1;
                                aux = rhs;
                                break;
                            case ElementSize.Composite:
                                startAndType = ((uint)start << 3) | 5;
                                aux = rhs & 7; // copy out the type bits, but not the count
                                header = segment[start];
                                aux |= ((uint)header) << 1;
                                dataWordsAndPointers = (uint)(header >> 32);
                                break;
                            default:
                                dataWordsAndPointers = 1;
                                startAndType = ((uint)start << 3) | 1;
                                aux = rhs;
                                break;
                        }
                        break;
                    default:
                        startAndType = lhs;
                        dataWordsAndPointers = 0;
                        aux = rhs;
                        break;
                }
            }
        }
        public override string ToString()
        {
            if (segment == null)
            {
                return "[nil]";
            }
            else if (startAndType == 0 && segment.Index == 0)
            {
                return "[root]";
            }
            else
            {
                switch (startAndType & 7)
                {
                    case Type.StructBasic:
                        return string.Format("[{0}:{1}-{2}] struct, {3} data words, {4} pointers",
                                segment.Index, startAndType >> 3,
                                (startAndType >> 3) + (dataWordsAndPointers & 0xFFFF) + (dataWordsAndPointers >> 16) - 1,
                                dataWordsAndPointers & 0xFFFF, dataWordsAndPointers >> 16);
                    case Type.ListBasic:
                    case Type.ListComposite:
                        int words, count = (int)(aux >> 3);
                        if (count == 0)
                        {
                            words = 0;
                        }
                        else
                        {
                            switch ((ElementSize)(aux & 7))
                            {

                                case ElementSize.EightBytesNonPointer:
                                case ElementSize.EightBytesPointer:
                                    words = count;
                                    break;
                                default:
                                    words = 0; // not yet calculated
                                    break;
                            }
                        }
                        return string.Format("[{0}:{1}-{2}] list, {3}, {4} items",
                                segment.Index, startAndType >> 3, (startAndType >> 3) + words - 1, (ElementSize)(aux & 7), aux >> 3);
                    case Type.FarSingle:
                        return string.Format("far to pad at [{0}:{1}]",
                                segment.Index, aux, startAndType >> 3);
                    case Type.Capability:
                        return string.Format("capability {0}", aux);
                    case Type.StructFragment:
                        return string.Format("[{0}:{1}-{1}] struct fragment, {2} bits, offset {3}",
                                segment.Index, startAndType >> 3, aux >> 16, aux & 0xFFFF);
                    case Type.FarDouble:
                        return string.Format("far to double-pad at [{0}:{1}]",
                                segment.Index, aux, startAndType >> 3);
                    default:
                        return "unknown";
                }
            }
        }
        private Pointer(ISegment segment, uint startAndType, uint dataWordsAndPointers, uint aux)
        {
            this.segment = segment;
            this.startAndType = startAndType;
            this.dataWordsAndPointers = dataWordsAndPointers;
            this.aux = aux;
        }
        private readonly ISegment segment;
        private readonly uint startAndType, dataWordsAndPointers, aux;
        // meaning:

        // structs:
        //  startAndType: LSB[type:2][fragment:1][start:29]MSB
        //  dataWordsAndPointers: LSB[data:16][pointers:16]MSB
        //  aux: LSB[shift:6][padding:10][size:5][padding:11]MSB // for use with fragments; note: padding here is for CPU efficiency only; could be relocated

        // lists:
        //  startAndType: LSB[type:2][composite:1][start:29]MSB
        //  dataWordsAndPointers: LSB[data:16][pointers:16]MSB
        //  aux: LSB[type:3][count:29]MSB

        // far:
        //  startAndType: LSB[type:2][double-far:1][start:29]MSB
        //  aux: LSB[segment:32]MSB

        // capability:
        //  startAndType: LSB[type:2][padding:30]MSB
        //  aux: index in capability table


        public bool GetBoolean(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = index % 8;
            return (word >> shift) != 0;
        }
        public byte GetByte(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index % 8) << 3; // 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((byte)(word >> shift));
        }
        public sbyte GetSByte(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index % 8) << 3; // 0 => 0, 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((sbyte)(word >> shift));
        }
        public short GetInt16(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index % 8) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((short)(word >> shift));
        }
        public ushort GetUInt16(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index % 8) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((ushort)(word >> shift));
        }

        public void SetUInt16(int index, ushort value)
        {
            int shift = (index % 8) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            SetDataWord(index >> 3, ((ulong)value) << shift, (ulong)0xFFFF << shift);
        }
        public int GetInt32(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index % 8) << 5; // 0 => 0, 1 => 32
            return unchecked((int)(word >> shift));
        }
        public uint GetUInt32(int index)
        {
            ulong word = GetDataWord(index >> 3);
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
        public void SetUInt64(int index, ulong value)
        {
            SetDataWord(index, value, ~(ulong)0);
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
            throw new NotImplementedException();
            //var segment = this.segment;            
            //if (segment == null) throw new NullReferenceException("Cannot dereference a nil pointer");
            //var header = this.header;
            //int wordIndex;
            //switch (header & 3)
            //{
            //    case 0: // struct-pointer
            //    case 1: // list-pointer
            //        return this;
            //    case 2: // far-pointer
            //        bool isFar = (header & 4) != 0;
            //        // unsigned
            //        wordIndex = unchecked((int)(((uint)header) >> 3));
            //        segment = segment.Message[checked((int)(header >> 32))];
            //        header = segment[wordIndex++];
            //        if (isFar)
            //        {
            //            if ((header & 3) != 2)
            //                throw new InvalidDataException("Far-pointer failed to resolve to second far-pointer");
            //            ulong tag = segment[wordIndex];
            //            if (((uint)tag & ~3) != 0)
            //                throw new InvalidDataException("Far-pointer expected zero offset");
            //            wordIndex = unchecked((int)(((uint)header) >> 3));
            //            segment = segment.Message[checked((int)(header >> 32))];
            //            header = tag;
            //        }
            //        switch (header & 3)
            //        {
            //            case 0:
            //            case 1:
            //                break;
            //            case 2:
            //                throw new InvalidDataException("Far-pointer resolved to unexpected far-pointer");
            //            case 3:
            //            default:
            //                throw new InvalidDataException("Far-pointer resolved to a capability pointer");
            //        }
            //        break;
            //    case 3: // capability-pointer
            //    default: // just to make the compiler happy
            //        throw new InvalidOperationException("Cannot deference a capability pointer");
            //}
            //int offset = unchecked(((int)header & ~3) >> 2);
            //return new Pointer(segment, header, wordIndex + offset);
        }

        const int MSB32 = 1 << 31;
        private ulong GetDataWord(int index)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            int count = (int)(dataWordsAndPointers & 0xFFFF);
                            if (index < count) return segment[(int)(startAndType >> 3) + index];
                            break;
                        case Type.ListBasic:
                            throw new NotImplementedException();
                        case Type.FarSingle:
                        case Type.FarDouble:
                            return Dereference().GetDataWord(index);
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            throw new NotImplementedException();
                        case Type.ListComposite:
                            throw new NotImplementedException();
                    }
                }
                return 0;
            }
        }
        private void SetDataWord(int index, ulong value, ulong mask)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            int count = (int)(dataWordsAndPointers & 0xFFFF);
                            if (index < count)
                            {
                                segment.SetValue((int)(startAndType >> 3) + index, value, mask);
                                return;
                            }
                            break;
                        case Type.ListBasic:
                            throw new NotImplementedException();
                        case Type.FarSingle:
                        case Type.FarDouble:
                            Dereference().SetDataWord(index, value, mask);
                            return;
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            throw new NotImplementedException();
                        case Type.ListComposite:
                            throw new NotImplementedException();
                    }
                }
                if (value != 0) throw CannotSetValue(index);
            }
        }
        static class Type
        {
            public const uint
                StructBasic = 0,
                ListBasic = 1,
                FarSingle = 2,
                Capability = 3,
                StructFragment = 4,
                ListComposite = 5,
                FarDouble = 6;
            // unused = 7
        }

        public Pointer GetPointer(int index)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            int count = (int)(dataWordsAndPointers >> 16);
                            if (index < count) return new Pointer(segment, (int)(startAndType >> 3) + (int)(dataWordsAndPointers & 0xFFFF) + index);
                            break;
                        case Type.ListBasic:
                            throw new NotImplementedException();
                        case Type.FarSingle:
                        case Type.FarDouble:
                            return Dereference().GetPointer(index);
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            throw new NotImplementedException();
                        case Type.ListComposite:
                            throw new NotImplementedException();
                    }
                }
                return default(Pointer);
            }
        }

        public void SetPointer(int index, Pointer value)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            int count = (int)(dataWordsAndPointers >> 16);
                            if (index < count)
                            {
                                value.WriteHeader(segment, (int)(startAndType >> 3) + (int)(dataWordsAndPointers & 0xFFFF) + index);
                                return;
                            }
                            break;
                        case Type.ListBasic:
                            throw new NotImplementedException();
                        case Type.FarSingle:
                        case Type.FarDouble:
                            Dereference().SetPointer(index, value);
                            return;
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            throw new NotImplementedException();
                        case Type.ListComposite:
                            throw new NotImplementedException();
                    }
                }
                if (value.IsValid) throw CannotSetValue(index);
            }
        }
        Exception CannotSetValue(int index)
        {
            if (!IsValid) return new InvalidOperationException("Cannot assign to a nil-pointer");
            if (index < 0) return new InvalidOperationException("Value cannot be assigned; this field is not defined because it is in a different 'union' partition");
            return new InvalidOperationException("Value cannot be assigned; the object has insufficient space for this field");
        }

        public bool IsValid { get { return segment != null && (startAndType != 0 || segment.Index != 0); } }

        public Pointer Allocate(short dataWords, short pointers)
        {
            uint newDataWordsAndPointers = ((uint)checked((ushort)dataWords)) | (((uint)checked((ushort)pointers)) << 16);
            return Allocate(dataWords + pointers, Type.StructBasic, newDataWordsAndPointers, newDataWordsAndPointers, 0);
        }
        private Pointer Allocate(int words, uint type, uint rhs, uint dataAndWords, uint aux)
        {
            unchecked
            {
                int start;
                var segment = this.segment;
                if (segment == null) throw new InvalidOperationException("A pointer withough a segment cannot be used to allocate");
                if (words < 0) throw new ArgumentOutOfRangeException("words");
                if (words == 0)
                {// takes no space; pretend it is at the end, but no need to allocate
                    start = segment.Length;
                    return new Pointer(segment, (uint)(start << 3) | (type & 7), dataAndWords, aux);
                }
                else if (segment.TryAllocate(words, out start))
                { // in-segment pointer
                    return new Pointer(segment, (uint)(start << 3) | (type & 7), dataAndWords, aux);
                }
                else
                {   // far pointer
                    int segIndex = segment.Index + 1;
                    var msg = segment.Message;
                    start = msg.Allocate(ref segIndex, words + 1); // leave an extra space for the header
                    msg[segIndex][start] = (ulong)(type & 3) | (((ulong)rhs) << 32); // zero offset, since data immediately follows header
                    return new Pointer(segment, (uint)(start << 3) | Type.FarSingle, 0, (uint)segIndex);
                }
            }
        }
        public Pointer Allocate(ElementSize elementSize, int length)
        {
            unchecked
            {
                if (length < 0) throw new ArgumentOutOfRangeException("length");
                int words;
                uint dataAndWords = 1;
                switch (elementSize)
                {
                    case ElementSize.ZeroByte:
                        words = 0;
                        break;
                    case ElementSize.OneBit:
                        words = ((length - 1) >> 6) + 1;
                        break;
                    case ElementSize.OneByte:
                        words = ((length - 1) >> 3) + 1;
                        break;
                    case ElementSize.TwoBytes:
                        words = ((length - 1) >> 2) + 1;
                        break;
                    case ElementSize.FourBytes:
                        words = ((length - 1) >> 1) + 1;
                        break;
                    case ElementSize.EightBytesPointer:
                        words = length;
                        dataAndWords = 1 << 16;
                        break;
                    case ElementSize.EightBytesNonPointer:
                        words = length;
                        break;
                    case ElementSize.Composite:
                        throw new InvalidOperationException("Composite lits should be allocated using the overload that accepts data word and pointer counts");
                    default:
                        throw new ArgumentOutOfRangeException("elementSize");

                }
                uint rhs = (uint)elementSize | ((uint)length << 3);
                return Allocate(words, Type.ListBasic, rhs, dataAndWords, rhs);
            }

        }

        internal void WriteHeader(ISegment targetSegment, int targetHeaderIndex)
        {
            unchecked
            {
                var segment = this.segment;
                ulong lhs, rhs;
                if (segment == null || (startAndType == 0 && segment.Index == 0))
                {
                    // nil-pointer
                    lhs = rhs = 0;
                }
                else if ((startAndType & 7) == Type.Capability)
                {   // doesn't care about segment
                    lhs = startAndType;
                    rhs = aux;
                }
                else if (targetSegment == segment || targetSegment.Index == segment.Index)
                {
                    bool isEmpty;
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            rhs = dataWordsAndPointers;
                            isEmpty = rhs == 0;
                            goto LocationBasedHeader;
                        case Type.ListBasic:
                            isEmpty = (aux >> 3) == 0;
                            rhs = aux;
                            goto LocationBasedHeader;
                        case Type.ListComposite:
                            isEmpty = false; // always a tag word
                            int itemCount = (int)(aux >> 3);
                            int itemSize = (int)((dataWordsAndPointers & 0xFFFF) + (uint)(dataWordsAndPointers >> 32));
                            rhs = (uint)((itemSize * itemCount) << 3) | (aux & 7);
                            goto LocationBasedHeader;
                        LocationBasedHeader:
                            // intra-segment pointer
                            int delta = isEmpty ? 0 : (((int)(startAndType >> 3) - targetHeaderIndex) - 1);
                            lhs = (startAndType & 3) | (uint)(delta << 2);
                            break;
                        case Type.StructFragment:
                            throw new InvalidOperationException("You cannot write a pointer to an struct that is a fragment of a list");
                        case Type.FarSingle:
                        case Type.FarDouble:
                            throw new NotImplementedException();
                        default:
                            throw new InvalidOperationException(); // huh?

                    }
                }
                else
                {
                    // inter-segment pointer
                    lhs = (uint)((startAndType & ~7) | 2);
                    rhs = (uint)segment.Index;
                }
                targetSegment[targetHeaderIndex] = (ulong)lhs | (((ulong)rhs) << 32);
            }
        }

        internal int SingleByteLength
        {
            get
            {
                switch (startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((aux & 3) == (uint)ElementSize.OneByte) return unchecked((int)aux >> 3);
                        break;
                    case Type.FarDouble:
                    case Type.FarSingle:
                        return Dereference().SingleByteLength;
                }
                throw SingleByteListExpected();
            }
        }
        static Exception SingleByteListExpected()
        {
            return new InvalidOperationException("Single-byte list pointer expected");
        }
        internal void AssertNilOrSingleByte()
        {
            if (IsValid)
            {
                switch (startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((aux & 3) == (uint)ElementSize.OneByte) return; // looks ok
                        break;
                    case Type.FarSingle:
                    case Type.FarDouble:
                        return; // defer judgement
                    case Type.StructBasic:
                    case Type.StructFragment:
                    case Type.ListComposite:
                    case Type.Capability:
                        break;
                }
                throw SingleByteListExpected();
            }
        }
    }
}
