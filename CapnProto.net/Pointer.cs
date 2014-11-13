
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
namespace CapnProto
{
    public struct Pointer : IEquatable<Pointer>, IComparable<Pointer>, IPointer
    {
        public override int GetHashCode()
        {
            ISegment seg = segment;
            unchecked
            {
                return Combine(Combine((int)startAndType, (int)dataWordsAndPointers), Combine((int)aux, seg == null ? 0 : seg.Index));
            }
        }
        Pointer IPointer.Pointer { get { return this; } }

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
        public static bool operator <(Pointer x, Pointer y) { return (Compare(x, y) & MSB32) != 0; }
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
        private Pointer(ISegment segment, int headerIndex) : this(segment, headerIndex, segment[headerIndex]) { }
        internal Pointer(ISegment segment, int headerIndex, ulong header)
        {
            unchecked
            {
                if (header == 0)
                {
                    if (headerIndex == 0 && segment != null && segment.Index == 0)
                    {
                        // smells like our root pointer hasn't been assigned yet...
                        // use a far-pointer to [0:0] for now
                        header = 2; // that's all we need!
                    }
                    else
                    {
                        this = default(Pointer); // nil pointer
                        return;
                    }
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
                                startAndType = ((uint)start << 3) | Type.ListBasic;
                                aux = rhs;
                                break;
                            case ElementSize.InlineComposite:
                                startAndType = ((uint)start << 3) | Type.ListComposite;
                                aux = rhs & 7; // copy out the type bits, but not the count
                                header = segment[start];
                                aux |= ((uint)header & ~(uint)3) << 1; // was offset 2, need 3
                                dataWordsAndPointers = (uint)(header >> 32);
                                break;
                            default:
                                dataWordsAndPointers = 1;
                                startAndType = ((uint)start << 3) | Type.ListBasic;
                                aux = rhs;
                                break;
                        }
                        break;
                    case Type.FarSingle:
                        int targetSegment = (int)rhs;
                        if (targetSegment != segment.Index) this.segment = segment = segment.Message[targetSegment];
                        startAndType = lhs;
                        dataWordsAndPointers = aux = 0;
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
            else if (startAndType == 2 && segment.Index == 0)
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
                        else if ((aux & 7) == (uint)ElementSize.InlineComposite)
                        {
                            words = 1 + ((int)(dataWordsAndPointers & 0xFFFF) + (int)(dataWordsAndPointers >> 16)) * count;
                        }
                        else
                        {
                            words = GetSpaceRequired((ElementSize)(aux & 7), count);
                        }
                        if (words == 0)
                        {
                            return string.Format("[{0}:{1}] nil list, {2}, {3} items",
                                segment.Index, startAndType >> 3, (ElementSize)(aux & 7), aux >> 3);
                        }
                        return string.Format("[{0}:{1}-{2}] list, {3}, {4} items ({5} words, {6} pointers each)",
                                segment.Index, startAndType >> 3, (startAndType >> 3) + words - 1, (ElementSize)(aux & 7), aux >> 3,
                                dataWordsAndPointers & 0xFFFF, dataWordsAndPointers >> 16);
                    case Type.FarSingle:
                        return string.Format("far to single-word pad at [{0}:{1}]", segment.Index, aux, startAndType >> 3);
                    case Type.Capability:
                        return string.Format("capability {0}", aux);
                    case Type.StructFragment:
                        return string.Format("[{0}:{1}-{1}] struct fragment, {2} bits, offset {3}",
                                segment.Index, startAndType >> 3, aux >> 16, aux & 0xFFFF);
                    case Type.FarDouble:
                        return string.Format("far to double-word pad at [{0}:{1}]",
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

        // struct:
        //  startAndType: LSB[type:2][fragment:1][start:29]MSB
        //  dataWordsAndPointers: LSB[data:16][pointers:16]MSB
        //  aux (fragment only): LSB[startBit:6][padding:2][size:6][padding:22]MSB 
        // note: ^^^ padding and layout here here is for CPU efficiency only; could be relocated, and size can
        // actually be stored in 3 bits, using it as a shift-multiplier i.e. size = 1 << [val]

        // lists:
        //  startAndType: LSB[type:2][composite:1][start:29]MSB
        //  dataWordsAndPointers: LSB[data:16][pointers:16]MSB
        //  aux: LSB[type:3][count:29]MSB

        // far:
        //  startAndType: LSB[type:2][double-far:1][start:29]MSB

        // capability:
        //  startAndType: LSB[type:2][padding:30]MSB
        //  aux: index in capability table


        public bool GetBoolean(int index)
        {
            ulong word = GetDataWord(index >> 6);
            int shift = index & 63;
            return ((word >> shift) & 1) != 0;
        }
        public void SetBoolean(int index, bool value)
        {
            int shift = index & 63;
            SetDataWord(index >> 6, value ? ((ulong)1 << shift) : (ulong)0, (ulong)1 << shift);
        }
        public byte GetByte(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index & 7) << 3; // 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((byte)(word >> shift));
        }
        public sbyte GetSByte(int index)
        {
            ulong word = GetDataWord(index >> 3);
            int shift = (index & 7) << 3; // 0 => 0, 1 => 8, 2 => 16, 3 => 32, ...
            return unchecked((sbyte)(word >> shift));
        }
        public void SetSByte(int index, sbyte value)
        {
            int shift = (index & 7) << 3; // 0 => 0, 1 => 8, 2 => 16, 3 => 32, ...
            SetDataWord(index >> 3, unchecked((ulong)value) << shift, (ulong)0xFF << shift);
        }
        public void SetByte(int index, byte value)
        {
            int shift = (index & 7) << 3; // 0 => 0, 1 => 8, 2 => 16, 3 => 32, ...
            SetDataWord(index >> 3, unchecked((ulong)value) << shift, (ulong)0xFF << shift);
        }
        public short GetInt16(int index)
        {
            ulong word = GetDataWord(index >> 2);
            int shift = (index & 3) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((short)(word >> shift));
        }
        public ushort GetUInt16(int index)
        {
            ulong word = GetDataWord(index >> 2);
            int shift = (index & 3) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            return unchecked((ushort)(word >> shift));
        }

        public void SetUInt16(int index, ushort value)
        {
            int shift = (index & 3) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            SetDataWord(index >> 2, unchecked((ulong)value) << shift, (ulong)0xFFFF << shift);
        }
        public void SetInt16(int index, short value)
        {
            int shift = (index & 3) << 4; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            SetDataWord(index >> 2, unchecked((ulong)value) << shift, (ulong)0xFFFF << shift);
        }
        public int GetInt32(int index)
        {
            ulong word = GetDataWord(index >> 1);
            int shift = (index & 1) << 5; // 0 => 0, 1 => 32
            return unchecked((int)(word >> shift));
        }
        public uint GetUInt32(int index)
        {
            ulong word = GetDataWord(index >> 1);
            int shift = (index & 1) << 5; // 0 => 0, 1 => 32
            return unchecked((uint)(word >> shift));
        }
        public void SetUInt32(int index, uint value)
        {
            int shift = (index & 1) << 5; // 0 => 0, 1 => 32
            SetDataWord(index >> 1, unchecked((ulong)value) << shift, (ulong)0xFFFFFFFF << shift);
        }
        public void SetInt32(int index, int value)
        {
            int shift = (index & 1) << 5; // 0 => 0, 1 => 16, 2 => 32, 3 => 48
            SetDataWord(index >> 1, unchecked((ulong)value) << shift, (ulong)0xFFFFFFFF << shift);
        }

        public void SetUInt64(int index, ulong value)
        {
            SetDataWord(index, value, ~(ulong)0);
        }
        public void SetInt64(int index, long value)
        {
            SetDataWord(index, unchecked((ulong)value), ~(ulong)0);
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
        public unsafe void SetSingle(int index, float value)
        {
            SetUInt32(index, *(uint*)(&value));
        }
        public unsafe double GetDouble(int index)
        {
            ulong val = GetDataWord(index);
            return *(double*)(&val);
        }

        public unsafe void SetDouble(int index, double value)
        {
            SetUInt64(index, *(ulong*)(&value));
        }

        public bool GetListBoolean(int index)
        {
            return GetDataWordInBasicList(index) != 0;
        }
        public void SetListBoolean(int index, bool value)
        {
            SetDataWordInBasicList(index, value ? (ulong)0 : (ulong)1, 0x01);
        }
        public byte GetListByte(int index)
        {
            return unchecked((byte)GetDataWordInBasicList(index));
        }
        public sbyte GetListSByte(int index)
        {
            return unchecked((sbyte)GetDataWordInBasicList(index));
        }
        public void SetListSByte(int index, sbyte value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFF);
        }
        public void SetListByte(int index, byte value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFF);
        }
        public short GetListInt16(int index)
        {
            return unchecked((short)GetDataWordInBasicList(index));
        }
        public ushort GetListUInt16(int index)
        {
            return unchecked((ushort)GetDataWordInBasicList(index));
        }

        public void SetListUInt16(int index, ushort value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFFFFFFFF);
        }
        public void SetListInt16(int index, short value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFFFF);
        }
        public int GetListInt32(int index)
        {
            return unchecked((int)GetDataWordInBasicList(index));
        }
        public uint GetListUInt32(int index)
        {
            return unchecked((uint)GetDataWordInBasicList(index));
        }
        public void SetListUInt32(int index, uint value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFFFFFFFF);
        }
        public void SetListInt32(int index, int value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), 0xFFFFFFFF);
        }

        public void SetListUInt64(int index, ulong value)
        {
            SetDataWordInBasicList(index, value, ~(ulong)0);
        }
        public void SetListInt64(int index, long value)
        {
            SetDataWordInBasicList(index, unchecked((ulong)value), ~(ulong)0);
        }
        public long GetListInt64(int index)
        {
            return unchecked((long)GetDataWordInBasicList(index));
        }
        public ulong GetListUInt64(int index)
        {
            return GetDataWordInBasicList(index);
        }
        public unsafe float GetListSingle(int index)
        {
            uint val = unchecked((uint)GetDataWordInBasicList(index));
            return *(float*)(&val);
        }
        public unsafe void SetListSingle(int index, float value)
        {
            SetDataWordInBasicList(index, *(uint*)(&value), 0xFFFFFFFF);
        }
        public unsafe double GetListDouble(int index)
        {
            ulong val = GetDataWordInBasicList(index);
            return *(double*)(&val);
        }

        public unsafe void SetListDouble(int index, double value)
        {
            SetDataWordInBasicList(index, *(ulong*)(&value), ~(ulong)0);
        }
        public Pointer Dereference()
        {
            unchecked
            {
                switch (startAndType & 7)
                {
                    case Type.FarSingle:
                        // the start refers to the header of the data
                        var ptr = new Pointer(segment, (int)startAndType >> 3);
                        if ((ptr.startAndType & 2) != 0) throw new InvalidOperationException("Single-hop far-pointer should have resolved to a struct or list");
                        return ptr;
                    case Type.FarDouble:
                        // the start refers to the double-word landing pad
                        int start = (int)(startAndType >> 3);
                        ulong innerHeader = segment[start++], dataHeader = segment[start];
                        if ((innerHeader & 7) != 2) throw new InvalidOperationException("Double-hop far-pointer should have landed on a single-hop landing-pad");
                        if ((dataHeader & 2) != 0) throw new InvalidOperationException("Double-hop far-pointer should have resolved to a struct or list");
                        return new Pointer(segment.Message[(int)(innerHeader >> 32)], (int)(innerHeader >> 3), dataHeader);
                }
            }
            return this;
        }

        const int MSB32 = 1 << 31;
        internal ulong GetDataWord(int index)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            if (index < (int)(dataWordsAndPointers & 0xFFFF)) return segment[(int)(startAndType >> 3) + index];
                            break;
                        case Type.ListBasic:
                            throw new InvalidOperationException("List values should be accessed via the GetList* methods or a list pointer");
                        case Type.FarSingle:
                        case Type.FarDouble:
                            return Dereference().GetDataWord(index);
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            // only one word
                            if (index == 0)
                            {
                                // k, let me walk you through this!
                                // startAndType >> 3 is the word-offset, so segment[(int)(startAndType >> 3)] is the entire word
                                // flags & 63 is the start-bit inside the word, so we can discard everything before that by shifting
                                // next we want to mask that with enough 1s for the size of the data, where the size is: (flags >> 8) & 63
                                // and a niec way of getting "n" 1s is to right-shift an all-1 block, then invert; all ones is: ~(ulong)0
                                // and so "n" 1s is ~( allones << n)
                                uint flags = aux;
                                return (segment[(int)(startAndType >> 3)] >> (int)(flags & 63)) & ~((~(ulong)0) << (int)((flags >> 8) & 63));
                            }
                            break;
                        case Type.ListComposite:
                            throw new InvalidOperationException("An inline-composite list should be accessed via struct pointers");
                    }
                }
                else if ((startAndType & 3) == 1) // any kind of list
                {
                    throw new IndexOutOfRangeException("index");
                }
                return 0;
            }
        }

        private ulong GetDataWordInBasicList(int index)
        {
            unchecked
            {
                switch (startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((index & MSB32) == 0 && index < (int)(aux >> 3))
                        {
                            int wordShift, valueShift;
                            ulong mask;
                            switch ((ElementSize)(aux & 7))
                            {
                                case ElementSize.OneBit: wordShift = 6; valueShift = (index & 63); mask = 1; break;
                                case ElementSize.OneByte: wordShift = 3; valueShift = (index & 7) << 3; mask = 0xFF; break;
                                case ElementSize.TwoBytes: wordShift = 2; valueShift = (index & 3) << 4; mask = 0xFFFF; break;
                                case ElementSize.FourBytes: wordShift = 1; valueShift = (index & 1) << 5; mask = 0xFFFFFFFF; break;
                                case ElementSize.EightBytesNonPointer:
                                    return segment[(int)(startAndType >> 3) + index];
                                case ElementSize.InlineComposite:
                                    throw new InvalidOperationException("An inline-composite list should be accessed via struct pointers");
                                case ElementSize.ZeroByte:
                                case ElementSize.EightBytesPointer:
                                default:
                                    return 0;
                            }
                            return (segment[(int)(startAndType >> 3) + (index >> wordShift)] >> valueShift) & mask;
                        }
                        throw new IndexOutOfRangeException("index");
                    case Type.FarSingle:
                    case Type.FarDouble:
                        return GetDataWordInBasicList(index);
                    default:
                        throw new InvalidOperationException();
                }
            }
        }
        internal void SetDataWord(int index, ulong value, ulong mask)
        {
            unchecked
            {
                if ((index & MSB32) == 0) // non-negative
                {
                    switch (startAndType & 7)
                    {
                        case Type.StructBasic:
                            if (index < (int)(dataWordsAndPointers & 0xFFFF))
                            {
                                segment.SetValue((int)(startAndType >> 3) + index, value, mask);
                                return;
                            }
                            break;
                        case Type.ListBasic:
                            throw new InvalidOperationException("List values should be accessed via the SetList* methods or a list pointer");
                        case Type.FarSingle:
                        case Type.FarDouble:
                            Dereference().SetDataWord(index, value, mask);
                            return;
                        case Type.Capability:
                            break;
                        case Type.StructFragment:
                            if (index == 0)
                            {
                                uint flags = aux;
                                int shift = (int)(flags & 63);
                                // need to check that the mask is entirely contained in the allowed region;
                                // if it isn't, the caller is attempting to overwrite another fragment;
                                // the legal mask is "n ones" (see GetDataWord),
                                // so we can & with "all ones except n zeros", which is (~0 << n)
                                if ((mask & ((~(ulong)0) << (int)((flags >> 8) & 63))) == 0)
                                {
                                    segment.SetValue((int)(startAndType >> 3), value << shift, mask << shift);
                                    return;
                                }
                            }
                            break;
                        case Type.ListComposite:
                            throw new InvalidOperationException("An inline-composite list should be accessed via struct pointers");
                    }
                }
                else if ((startAndType & 3) == 1) // any kind of list
                {
                    throw new IndexOutOfRangeException("index");
                }
                if (value != 0) throw CannotSetValue(index);
            }
        }

        private void SetDataWordInBasicList(int index, ulong value, ulong mask)
        {
            unchecked
            {
                switch (startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((index & MSB32) == 0 && index < (int)(aux >> 3))
                        {
                            int wordShift, valueShift;
                            ulong localMask;
                            switch ((ElementSize)(aux & 7))
                            {
                                case ElementSize.OneBit: wordShift = 6; valueShift = (index & 63); localMask = 1; break;
                                case ElementSize.OneByte: wordShift = 3; valueShift = (index & 7) << 3; localMask = 0xFF; break;
                                case ElementSize.TwoBytes: wordShift = 2; valueShift = (index & 3) << 4; localMask = 0xFFFF; break;
                                case ElementSize.FourBytes: wordShift = 1; valueShift = (index & 1) << 5; localMask = 0xFFFFFFFF; break;
                                case ElementSize.EightBytesNonPointer:
                                    segment.SetValue((int)(startAndType >> 3) + index, value, mask);
                                    return;
                                case ElementSize.InlineComposite:
                                    throw new InvalidOperationException("An inline-composite list should be accessed via struct pointers");
                                case ElementSize.ZeroByte:
                                case ElementSize.EightBytesPointer:
                                default:
                                    if (value != 0) throw CannotSetValue(index);
                                    return;
                            }
                            if ((mask & ~localMask) == 0)
                            { // check not attempting to overwrite
                                segment.SetValue((int)(startAndType >> 3) + (index >> wordShift), value << valueShift, mask << valueShift);
                                return;
                            }
                        }
                        throw new IndexOutOfRangeException("index");
                    case Type.FarDouble:
                    case Type.FarSingle:
                        Dereference().SetDataWordInBasicList(index, value, mask);
                        return;
                    default:
                        throw new InvalidOperationException();
                }
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
        public Pointer GetListPointer(int index)
        {
            // specifically refers to basic lists of pointers; cannot be used in another other context
            unchecked
            {
                switch(startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((aux & 7) == (uint)ElementSize.EightBytesPointer)
                        {
                            if ((index & MSB32) == 0 && index < (int)(aux >> 3))
                            {
                                return new Pointer(segment, (int)(startAndType >> 3) + index);
                            }
                            throw new IndexOutOfRangeException();
                        }
                        break;
                    case Type.FarDouble:
                    case Type.FarSingle:
                        return Dereference().GetListPointer(index);
                   
                }
                throw new InvalidOperationException();
            }
        }
        public void SetListPointer(int index, Pointer value)
        {
            // specifically refers to basic lists of pointers; cannot be used in another other context
            unchecked
            {
                switch (startAndType & 7)
                {
                    case Type.ListBasic:
                        if ((aux & 7) == (uint)ElementSize.EightBytesPointer)
                        {
                            if ((index & MSB32) == 0 && index < (int)(aux >> 3))
                            {
                                value.WriteHeader(segment, (int)(startAndType >> 3) + index);
                                return;
                            }
                            throw new IndexOutOfRangeException();
                        }
                        break;
                    case Type.FarDouble:
                    case Type.FarSingle:
                        Dereference().SetListPointer(index, value);
                        return;

                }
                throw new InvalidOperationException();
            }
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
                            if (index >= (int)(aux >> 3)) throw new IndexOutOfRangeException("index");
                            return GetPointerInBasicList(index);
                        case Type.FarSingle:
                        case Type.FarDouble:
                            return Dereference().GetPointer(index);
                        case Type.Capability: // not even data...
                        case Type.StructFragment: // never has pointers
                            break;
                        case Type.ListComposite:
                            // non-fragment hook into middle of a list; dataWordsAndPointers is copied verbatim; offset is tweaked, aux is nil
                            if (index >= (int)(aux >> 3)) throw new IndexOutOfRangeException("index");

                            int offset = 1 + (int)(startAndType >> 3) + index * ((int)(dataWordsAndPointers & 0xFFFF) + (int)(dataWordsAndPointers >> 16));
                            return new Pointer(segment, (uint)(offset << 3), dataWordsAndPointers, 0);
                    }
                }
                else if ((startAndType & 3) == 1) // any kind of list
                {
                    throw new IndexOutOfRangeException("index");
                }
                return default(Pointer);
            }
        }

        private Pointer GetPointerInBasicList(int index)
        {
            unchecked
            {
                if ((startAndType & 7) != Type.ListBasic) throw new InvalidOperationException();
                if ((index & MSB32) == 0 && index <= (int)(aux >> 3))
                {

                    int word, size, startBit;
                    switch ((ElementSize)(aux & 7))
                    {
                        case ElementSize.ZeroByte:
                            // can be treated as a simple (non-fragment) pointer without any words
                            return new Pointer(segment, startAndType & ~(uint)7, 0, 0);
                        case ElementSize.OneBit:
                            size = 1;
                            word = index >> 6;
                            startBit = index & 63;
                            break;
                        case ElementSize.OneByte:
                            size = 8;
                            word = index >> 3;
                            startBit = (index & 7) << 3;
                            break;
                        case ElementSize.TwoBytes:
                            size = 16;
                            word = index >> 2;
                            startBit = (index & 3) << 4;
                            break;
                        case ElementSize.FourBytes:
                            size = 32;
                            word = index >> 1;
                            startBit = (index & 1) << 5;
                            break;
                        case ElementSize.EightBytesPointer:
                            // can be treated as a simple (non-fragment) pointer with a single pointer word
                            return new Pointer(segment, (uint)(((int)(startAndType >> 3) + index) << 3), 1 << 16, 0);
                        case ElementSize.EightBytesNonPointer:
                            // can be treated as a simple (non-fragment) pointer with a single data word
                            return new Pointer(segment, (uint)(((int)(startAndType >> 3) + index) << 3), 1, 0);
                        default:
                            return default(Pointer); // I have no clue what the hell you want!
                    }
                    return new Pointer(segment, (uint)(((startAndType >> 3) + word) << 3) | Type.StructFragment, 1, (uint)(startBit) | (uint)(size << 8));
                }
                throw new IndexOutOfRangeException("index");
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
                            if (value.Dereference() != GetPointerInBasicList(index))
                                throw new InvalidOperationException("You cannot reassign a pointer that refers to a location inside a struct-fragment list");
                            return;
                        case Type.FarSingle:
                        case Type.FarDouble:
                            Dereference().SetPointer(index, value);
                            return;
                        case Type.Capability: // not even data...
                        case Type.StructFragment: // fragments never have pointers
                            break;
                        case Type.ListComposite:
                            if (index >= (int)(aux >> 3)) throw new IndexOutOfRangeException("index");
                            int offset = 1 + (int)(startAndType >> 3) + index * ((int)(dataWordsAndPointers & 0xFFFF) + (int)(dataWordsAndPointers >> 16));
                            var expected = new Pointer(segment, (uint)(offset << 3), dataWordsAndPointers, 0);
                            if (value.Dereference() != expected)
                                throw new InvalidOperationException("You cannot reassign a pointer that refers to a location inside a composite/inline list");
                            return;
                    }
                }
                else if ((startAndType & 3) == 1) // any kind of list
                {
                    throw new IndexOutOfRangeException("index");
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

        public bool IsValid { get { return segment != null; } }

        public T Allocate<T>() where T : struct, IPointer
        {
            return TypeAccessor<T>.Instance.Create(this);
        }
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
                if (segment == null) throw new InvalidOperationException("A null-pointer cannot be used to allocate");
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
                {   // far pointer; will need the data and a header; ideally, in the same block
                    bool plusHeader = true;
                    var msg = segment.Message;
                    start = msg.Allocate(ref segment, ref plusHeader, words);
                    if (plusHeader)
                    {
                        // the header and data fit together
                        segment[start] = (ulong)(type & 3) | (((ulong)rhs) << 32); // zero offset, since data immediately follows header
                        return new Pointer(segment, (uint)(start << 3) | Type.FarSingle, 0, 0);
                    }
                    uint dataSegmentIndex = (uint)segment.Index;
                    // double-far landing pad is 2 bytes; note we know that plusHeader is false here
                    int headerStart = msg.Allocate(ref segment, ref plusHeader, 2);
                    segment[headerStart] = (uint)(start << 3) | Type.FarSingle | (((ulong)dataSegmentIndex) << 32);
                    segment[headerStart + 1] = (ulong)(type & 3) | (((ulong)rhs) << 32);
                    return new Pointer(segment, (uint)(headerStart << 3) | Type.FarDouble, 0, 0);
                }
            }
        }
        public Pointer AllocateList(short dataWords, short pointers, int length)
        {
            unchecked
            {
                if (dataWords < 0) throw new ArgumentOutOfRangeException("dataWords");
                if (pointers < 0) throw new ArgumentOutOfRangeException("pointers");
                if (length < 0) throw new ArgumentOutOfRangeException("length");

                int wordsPerElement = (int)dataWords + (int)pointers;
                int totalWords = wordsPerElement * length;
                uint rhs = (uint)(totalWords << 3 | (int)ElementSize.InlineComposite);
                uint aux = (uint)(length << 3 | (int)ElementSize.InlineComposite);
                uint dataAndWords = (uint)(ushort)dataWords | (((uint)(ushort)pointers) << 16);
                var list = Allocate(totalWords + 1, (uint)Type.ListComposite, rhs, dataAndWords, aux);
                list.SetTagWord((uint)(length << 2) | Type.StructBasic, dataAndWords);
                return list;

            }
        }

        private void SetTagWord(uint lhs, uint rhs)
        {
            switch (startAndType & 7)
            {
                case Type.FarSingle:
                case Type.FarDouble:
                    Dereference().SetTagWord(lhs, rhs);
                    break;
                case Type.ListComposite:
                    segment[(int)(startAndType >> 3)] = (ulong)lhs | ((ulong)rhs << 32);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
        static int GetSpaceRequired(ElementSize elementSize, int length)
        {
            switch (elementSize)
            {
                case ElementSize.ZeroByte: return 0;
                case ElementSize.OneBit: return ((length - 1) >> 6) + 1;
                case ElementSize.OneByte: return ((length - 1) >> 3) + 1;
                case ElementSize.TwoBytes: return ((length - 1) >> 2) + 1;
                case ElementSize.FourBytes: return ((length - 1) >> 1) + 1;
                case ElementSize.EightBytesPointer: return length;
                case ElementSize.EightBytesNonPointer: return length;
                default:
                    throw new ArgumentOutOfRangeException("elementSize");
            }
        }
        public Pointer AllocateList(ElementSize elementSize, int length)
        {
            unchecked
            {
                if (length < 0) throw new ArgumentOutOfRangeException("length");
                int words;
                uint dataAndWords = 1;
                switch (elementSize)
                {
                    case ElementSize.ZeroByte:
                    case ElementSize.OneBit:
                    case ElementSize.OneByte:
                    case ElementSize.TwoBytes:
                    case ElementSize.FourBytes:
                    case ElementSize.EightBytesNonPointer:
                        words = GetSpaceRequired(elementSize, length);
                        break;
                    case ElementSize.EightBytesPointer:
                        words = GetSpaceRequired(elementSize, length);
                        dataAndWords = 1 << 16;
                        break;
                    case ElementSize.InlineComposite:
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
                if (segment == null)
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
                        if ((aux & 7) == (uint)ElementSize.OneByte) return unchecked((int)aux >> 3);
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
                        if ((aux & 7) == (uint)ElementSize.OneByte) return; // looks ok
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
        internal string ReadString()
        {
            if (segment == null) return null;
            switch (startAndType & 7)
            {
                case Type.ListBasic:
                    if ((aux & 7) == (uint)ElementSize.OneByte)
                    {
                        unchecked
                        {
                            return segment.ReadString((int)(startAndType >> 3), ((int)aux >> 3));
                        }
                    }
                    break;
                case Type.FarSingle:
                case Type.FarDouble:
                    return Dereference().ReadString();
            }
            throw SingleByteListExpected();
        }
        internal void ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int words)
        {
            switch (startAndType & 7)
            {
                case Type.ListBasic:
                    if ((aux & 7) == (uint)ElementSize.OneByte && segment != null)
                    {
                        if (segment.ReadWords((int)(startAndType >> 3) + wordOffset, buffer, bufferOffset, words) != words)
                            throw new EndOfStreamException();
                        return;
                    }
                    break;
                case Type.FarSingle:
                case Type.FarDouble:
                    Dereference().ReadWords(wordOffset, buffer, bufferOffset, words);
                    return;
            }
            throw SingleByteListExpected();
        }
        internal void WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int words)
        {
            switch (startAndType & 7)
            {
                case Type.ListBasic:
                    if ((aux & 7) == (uint)ElementSize.OneByte && segment != null)
                    {
                        if (segment.WriteWords((int)(startAndType >> 3) + wordOffset, buffer, bufferOffset, words) != words)
                            throw new EndOfStreamException();
                        return;
                    }
                    break;
                case Type.FarSingle:
                case Type.FarDouble:
                    Dereference().WriteWords(wordOffset, buffer, bufferOffset, words);
                    return;
            }
            throw SingleByteListExpected();
        }
        internal void WriteString(string value)
        {
            switch (startAndType & 7)
            {
                case Type.ListBasic:
                    if ((aux & 7) == (uint)ElementSize.OneByte)
                    {
                        unchecked
                        {
                            int len = ((int)aux >> 3);
                            int bytes = segment.WriteString((int)(startAndType >> 3), value, len);
                            if (bytes != len - 1)
                                throw new InvalidOperationException("String written with incorrect length");
                            return;
                        }
                    }
                    break;
                case Type.FarSingle:
                case Type.FarDouble:
                    Dereference().WriteString(value);
                    return;
            }
            throw SingleByteListExpected();
        }


        public int Count()
        {
            switch (startAndType & 7)
            {
                case Type.ListBasic:
                case Type.ListComposite:
                    return unchecked((int)(aux >> 3));
                case Type.FarDouble:
                case Type.FarSingle:
                    return Dereference().Count();
            }
            throw ListExpected();
        }
        Exception ListExpected()
        {
            return new InvalidOperationException("A list was expected; " + ToString());
        }
        public bool IsFar
        {
            get { return (startAndType & 3) == 2; }
        }
        public int Pointers()
        {   
            switch(startAndType & 7)
            {
                case Type.StructBasic:
                    return (int)(dataWordsAndPointers >> 16);
                case Type.StructFragment:
                    return 0; // never has pointers
                case Type.ListBasic:
                case Type.ListComposite:
                    return 0;
                case Type.FarSingle:
                case Type.FarDouble:
                    return Dereference().Pointers();
                default:
                    return 0;
            }
        }

        public int DataWords()
        {
            switch (startAndType & 3)
            {
                case Type.StructBasic:
                    return (int)(dataWordsAndPointers & 0xFFFF);
                    // note StructFragment never has pointers
                case Type.FarSingle:
                    return Dereference().DataWords();
                default:
                    return 0;
            }
        }

        public bool IsList()
        {
            switch(startAndType & 3)
            {
                case Type.ListBasic:
                case Type.ListComposite:
                    return true;
                case Type.FarDouble:
                case Type.FarSingle:
                    return Dereference().IsList();
                default:
                    return false;
            }
        }

        internal bool IsComplexList()
        {
            switch(startAndType & 7)
            {
                case Type.ListComposite:
                    return true;
                case Type.ListBasic:
                    return (aux & 7) == (uint)ElementSize.EightBytesPointer;
                case Type.FarDouble:
                case Type.FarSingle:
                    return Dereference().IsComplexList();
                default:
                    return false;
            }
        }
    }
}
