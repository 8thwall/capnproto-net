using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CapnProto
{
    abstract public partial class CapnProtoReader : IDisposable
    {

        public void Crawl(TextWriter output)
        {
            var segments = GetSegments();
            output.WriteLine("segments: {0}", segments.Length);
            for(int i = 0;i < segments.Length; i++)
            {
                output.WriteLine("segment {0}: {1} words", i, segments[i]);
            }

            var pending = new SortedDictionary<PendingPointer, object>();
            var ptr = ReadWord(0, 0);
            pending.Add(new PendingPointer(0, 0, 1, ptr), null);

            var keys = pending.Keys;
            int lastSegment = 0, lastOffset = 0;
            while(true)
            {
                PendingPointer next;
                using (var iter = keys.GetEnumerator())
                {
                    if (!iter.MoveNext()) break;
                    next = iter.Current;
                }
                pending.Remove(next);

                output.WriteLine("{0}/{1}: {2}", next.Segment, next.EffectiveOffset, PointerType.GetName(next.Pointer));

                if (next.Segment < lastSegment || (next.Segment == lastSegment && next.EffectiveOffset < lastOffset))
                {
                    output.WriteLine("Moving backwards!");
                    break;
                }
                lastSegment = next.Segment;
                lastOffset = next.EffectiveOffset;
                

                switch(next.Pointer & PointerType.Mask)
                {
                    case PointerType.Struct:
                        {
                            int data = unchecked((int)(ushort)(next.Pointer >> 32)),
                                pointers = unchecked((int)(ushort)(next.Pointer >> 48));
                            output.WriteLine("\tdata: {0}; pointers: {1}", data, pointers);
                            for (int i = 0; i < pointers; i++)
                            {
                                ProcessPointer(output, pending, i, next, next.EffectiveOffset + data + i);
                            }
                        }
                        break;
                    case PointerType.List:
                        var rhs = (uint)(next.Pointer >> 32);
                        var itemType = unchecked((ElementSize)(int)(rhs & 7));
                        var size = unchecked((int)(rhs >> 3));
                        output.WriteLine("\ttype: {0}; size: {1}", itemType, size);
                        switch(itemType)
                        {
                            case ElementSize.EightBytesPointer:
                                for(int i = 0; i < size; i++)
                                {
                                    ProcessPointer(output, pending, i, next, next.EffectiveOffset + i);
                                }
                                break;
                            case ElementSize.Composite:
                                var tagWord = ReadWord(next.Segment, next.EffectiveOffset);
                                int data = unchecked((int)(ushort)(tagWord >> 32)),
                                    pointers = unchecked((int)(ushort)(tagWord >> 48)),
                                    itemCount = unchecked((int)(((uint)tagWord) >> 2)),
                                    itemSize = data + pointers;
                                ulong itemPointer = tagWord & 0xFFFFFFFF00000003;
                                output.WriteLine("\tdata: {0}; pointers: {1}; items: {2} (each {3} words)", data, pointers, itemCount, itemSize);
                                if (pointers != 0)
                                {
                                    int offset = next.EffectiveOffset + 1;
                                    for (int i = 0; i < itemCount; i++)
                                    {
                                        offset += data;
                                        for (int j = 0; j < pointers; j++)
                                        {
                                            ProcessPointer(output, pending, j, next, offset++);
                                        }
                                    }
                                }
                                break;
                        }
                        break;
                    case PointerType.Far:
                        if((next.Pointer & 4) != 0)
                        {
                            // double-landing pad
                            throw new NotSupportedException("double landing-pad not supported for crawl");
                        } else
                        {
                            ProcessPointer(output, pending, 0, next, next.EffectiveOffset);
                        }
                        break;
                }
            }
        }

        private void ProcessPointer(TextWriter output, SortedDictionary<PendingPointer, object> pending, int index, PendingPointer parent, int offset)
        {
            ulong ptr;
            ptr = ReadWord(parent.Segment, offset);
            if (ptr == 0)
            {
                output.WriteLine("\t\t{0}: [nil]", index);
            }
            else
            {
                var newPtr = new PendingPointer(parent.Segment, parent.EffectiveOffset, offset + 1, ptr);
                output.WriteLine("\t\t{0}: {1} at {2}/{3}", index, PointerType.GetName(ptr), newPtr.Segment, newPtr.EffectiveOffset);
                if (pending.ContainsKey(newPtr))
                {
                    output.WriteLine("\t\t\tduplicated key");
                    foreach (PendingPointer key in pending.Keys)
                    {
                        if(key.Equals(newPtr))
                        {
                            output.WriteLine("\t\t\talso referenced from {0}/{1}", key.ParentSegment, key.ParentOffset);
                        }
                    }                    
                }
                else
                {
                    pending.Add(newPtr, null);
                }
            }
        }

        protected const int ScratchLengthBytes = 64, ScratchLengthWords = ScratchLengthBytes / 8; // must be at least 8
        protected readonly byte[] Scratch = new byte[ScratchLengthBytes];
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



        public abstract void ReadWords(int segment, int wordOffset, byte[] buffer, int bufferOffset, int wordCount);
        public virtual ulong ReadWord(int segment, int wordOffset)
        {
            var tmp = Scratch;
            ReadWords(segment, wordOffset, tmp, 0, 1);
            return BitConverter.ToUInt64(tmp, 0);
        }

        private int AssertListHeader(ulong pointer, ref int origin, ElementSize expectedSize)
        {
            ElementSize actualSize;
            int count = ListPointer.Parse(pointer, ref origin, out actualSize);
            if (expectedSize != actualSize) throw new InvalidOperationException("Invalid list pointer size; expected " + expectedSize.ToString() + ", found " + actualSize.ToString());
            return count;
        }

        public virtual unsafe List<string> ReadStringList(int segment, int origin, ulong pointer)
        {
            int count = AssertListHeader(pointer, ref origin, ElementSize.EightBytesPointer);
            TypeModel.Log("Reading string; using {0} stack words", count);
            ulong* raw = stackalloc ulong[count];
            ReadWords(segment, origin, count, count, raw);
            var list = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(ReadStringFromPointer(segment, origin + i, raw[i]));
            }
            return list;
        }

        public virtual unsafe List<T> ReadStructList<T>(DeserializationContext context, int segment, int origin, ulong pointer) where T : IBlittable
        {
            ElementSize size;
            if ((pointer & PointerType.Mask) == PointerType.Far)
            {
                TypeModel.Log("{0}: de-referencing from {1}/{2}", PointerType.GetName(pointer), segment, origin);
                pointer = ParseFarPointer(pointer, ref segment, ref origin);
                TypeModel.Log("{0}: de-referenced to {1}/{2}", PointerType.GetName(pointer), segment, origin);
            }
            int count = ListPointer.Parse(pointer, ref origin, out size);
            if (count == 0) return new List<T>();
            TypeModel.Log("{0}: Deserializing {1} list of {2} from {3}/{4} (size {5})",
                context.Depth, size, typeof(T).Name, segment, origin, count);
            switch (size)
            {
                case ElementSize.EightBytesPointer:
                    {
                        TypeModel.Log("Reading bulk pointers; using {0} stack words", count);
                        ulong* raw = stackalloc ulong[count];
                        ReadWords(segment, origin, count, count, raw);
                        var list = new List<T>(count);
                        var model = context.Model;
                        for (int i = 0; i < count; i++)
                        {
                            TypeModel.Log("{0}: element {1} of {2} at {3}/{4}", context.Depth, i, count, segment, origin);
                            list.Add(model.Deserialize<T>(segment, origin + i, context, raw[i]));
                        }
                        return list;
                    }
                case ElementSize.Composite:
                    {
                        ulong tag = context.Reader.ReadWord(segment, origin++);
                        // The tag has the same layout as a struct pointer, except that the pointer offset (B)
                        // instead indicates the number of elements in the list.
                        // Meanwhile, section (D) of the list pointer – which normally would store this element count
                        // – instead stores the total number of words in the list (not counting the tag word).
                        if ((tag & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Struct-based list-tag expected");
                        uint first = (uint)tag, second = (uint)(tag >> 32);
                        int numberOfElements = unchecked((int)(first >> 2));
                        int dataWords = unchecked((int)(second & 0xFFFF)),
                            ptrWords = unchecked((int)((second >> 16) & 0xFFFF));
                        int elementSize = dataWords + ptrWords;
                        var list = new List<T>(numberOfElements);
                        ulong elementPointer = tag & 0xFFFFFFFF00000003;
                        // just want the data/ptr headers and type;
                        // offset will always be zero relative to origin
                        var model = context.Model;
                        for (int i = 0; i < numberOfElements; i++)
                        {
                            TypeModel.Log("{0}: element {1} of {2} at {3}/{4}", context.Depth, i, numberOfElements, segment, origin);
                            T newElement = model.Deserialize<T>(segment, origin, context, elementPointer);
                            list.Add(newElement);
                            origin += elementSize;
                        }
                        return list;
                    }
                default:
                    throw new NotImplementedException("List size not implemented: " + size);
            }
        }
        // lsb                        far pointer                        msb
        // +-+-+---------------------------+-------------------------------+
        // |A|B|            C              |               D               |
        // +-+-+---------------------------+-------------------------------+
        // A (2 bits) = 2, to indicate that this is a far pointer.
        // B (1 bit) = 0 if the landing pad is one word, 1 if it is two words.
        // C (29 bits) = Offset, in words, from the start of the target segment to the location of the far-pointer landing-pad within that segment.  Unsigned.
        // D (32 bits) = ID of the target segment.  (Segments are numbered sequentially starting from zero.)
        private ulong ParseFarPointer(ulong pointer, ref int segment, ref int origin)
        {
            int newSegment, newOrigin;
            bool doubleLandingPad = DereferenceFarPointer(pointer, out newSegment, out newOrigin);
            TypeModel.Log("far-pointer resolved to landing-pad at {0}/{1}", newSegment, newOrigin);
            if (doubleLandingPad)
            {
                // double landing-pad; the next word is the object-header, where-as
                // the contents of the landing-pad are a landing-pad to the *content*
                var tagWord = ReadWord(segment, origin + 1);
                pointer = ReadWord(newSegment, newOrigin);
                if(DereferenceFarPointer(pointer, out newSegment, out newOrigin))
                {
                    throw new InvalidOperationException("Triple landing-pads should not exist");
                }
                TypeModel.Log("far-pointer resolved to landing-pad at {0}/{1}", newSegment, newOrigin);
                return tagWord;
            }
            else
            {
                // basic landing-pad
                segment = newSegment;
                origin = newOrigin;
                return ReadWord(segment, origin++);
            }
        }
        static bool DereferenceFarPointer(ulong pointer, out int segment, out int origin)
        {
            if ((pointer & PointerType.Mask) != PointerType.Far) throw new InvalidOperationException("Expected far pointer");
            segment = unchecked((int)(pointer >> 32));
            origin = unchecked((int)(((uint)pointer) >> 3));
            return (pointer & 4) != 0;
        }
        public virtual unsafe List<long> ReadInt64List(int segment, int origin, ulong pointer)
        {
            int count = AssertListHeader(pointer, ref origin, ElementSize.EightBytesNonPointer);
            TypeModel.Log("Reading uints; using {0} stack words", count);
            ulong* raw = stackalloc ulong[count];
            ReadWords(segment, origin, count, count, raw);
            var list = new List<long>(count);
            for (int i = 0; i < count; i++)
            {
                list.Add(unchecked((long)raw[i]));
            }
            return list;
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
        public byte[] ReadBytesFromPointer(int segment, int wordOffset, ulong ptr)
        {
            if ((ptr & PointerType.Mask) == PointerType.Far)
            {
                ptr = ParseFarPointer(ptr, ref segment, ref wordOffset);
            }
            var lptr = new ListPointer(ptr);
            if (lptr.ElementSize != ElementSize.OneByte) throw new InvalidOperationException("Expected pointer to single-byte data");

            int len = lptr.Size;
            if (len == 0) return NixBytes;
            return ReadBytes(segment, wordOffset + lptr.Offset, len);
        }
        static readonly byte[] NixBytes = new byte[0];
        public string ReadStringFromPointer(int segment, int wordOffset, ulong ptr)
        {
            if((ptr & PointerType.Mask) == PointerType.Far)
            {
                ptr = ParseFarPointer(ptr, ref segment, ref wordOffset);
            }
            var lptr = new ListPointer(ptr);
            if (lptr.ElementSize != ElementSize.OneByte) throw new InvalidOperationException("Expected pointer to single-byte data");

            int len = lptr.Size;
            if (len == 0) throw ExpectedNullTerminatedString();

            if (len <= ScratchLengthBytes)
            {
                var tmp = Scratch;
                ReadWords(segment, wordOffset + lptr.Offset, tmp, 0, ToWords(len));
                if (tmp[len - 1] != 0) throw ExpectedNullTerminatedString();

                return Encoding.UTF8.GetString(tmp, 0, len - 1);

            }
            return ReadString(segment, wordOffset + lptr.Offset, len);
        }
        private static int ToWords(int bytes)
        {
            return (bytes + 7) / 8;
        }
        protected Exception ExpectedNullTerminatedString()
        {
            return new InvalidOperationException("Expected nul-terminated string");
        }

        protected abstract string ReadString(int segment, int wordOffset, int count);
        protected abstract byte[] ReadBytes(int segment, int wordOffset, int count);

        private SegmentRange firstSegment;
        private SegmentRange[] otherSegments;
        public virtual void ReadPreamble()
        {
            int offset = 0;
            var word = ReadWord(0, offset++);
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
                    word = ReadWord(0, offset++);
                    otherSegments[index++] = new SegmentRange(ref segmentOffset, (int)word);
                    otherSegments[index++] = new SegmentRange(ref segmentOffset, (int)(word >> 32));
                }

                if ((additionalSegments % 2) != 0)
                {
                    word = ReadWord(0, offset++);
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

        //lsb                      struct pointer                       msb
        //+-+-----------------------------+---------------+---------------+
        //|A|             B               |       C       |       D       |
        //+-+-----------------------------+---------------+---------------+
        //A(2 bits) = 0, to indicate that this is a struct pointer.
        //B(30 bits) = Offset, in words, from the end of the pointer to the
        //    start of the struct's data section.  Signed.
        //C(16 bits) = Size of the struct's data section, in words.
        //D(16 bits) = Size of the struct's pointer section, in words.
        public unsafe void ReadData(int segment, int origin, ulong pointer, ulong* raw, int max)
        {
            if ((pointer & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Expected struct pointer");
            var offset = unchecked(((int)pointer) >> 2); // note: int because signed
            var count = (int)((pointer >> 32) & 0xFFFF);
            TypeModel.Log("reading data payload from {0}/{1}: {2} words", segment, origin + offset, Math.Max(count, max));
            ReadWords(segment, origin + offset, count, max, raw);
        }

        public unsafe int ReadPointers(int segment, int origin, ulong pointer, ulong* raw, int max)
        {
            if ((pointer & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Expected struct pointer");
            var offset = unchecked(((int)pointer) >> 2); // note: int because signed
            var dataCount = (int)((pointer >> 32) & 0xFFFF);
            var count = (int)((pointer >> 48) & 0xFFFF);

            int pointerBase = origin + offset + dataCount;
            TypeModel.Log("reading pointers from {0}/{1}: {2} words", segment, pointerBase, Math.Max(count, max));
            ReadWords(segment, pointerBase, count, max, raw);
            return pointerBase;
        }

        protected virtual unsafe void ReadWords(int segment, int origin, int available, int expected, ulong* raw)
        {
            int wordsToRead = Math.Max(available, expected);

            var buffer = Scratch;
            int offset = 0;
            while (wordsToRead >= ScratchLengthWords)
            {
                ReadWords(segment, origin + offset, buffer, 0, ScratchLengthWords);
                Marshal.Copy(buffer, 0, new IntPtr(raw + offset), ScratchLengthBytes);
                offset += ScratchLengthWords;
                wordsToRead -= ScratchLengthWords;
            }
            if (wordsToRead > 0)
            {
                ReadWords(segment, origin + offset, buffer, 0, wordsToRead);
                Marshal.Copy(buffer, 0, new IntPtr(raw + offset), wordsToRead * 8);
            }
            // zero out any words that didn't receive data
            for (int i = available; i < expected; i++)
                raw[i] = 0;
        }

        public int[] GetSegments()
        {
            int count = 1 + (otherSegments == null ? 0 : otherSegments.Length);
            int[] lengths = new int[count];
            lengths[0] = firstSegment.Length;
            if (otherSegments != null)
            {
                for (int i = 0; i < otherSegments.Length; i++)
                {
                    lengths[i + 1] = otherSegments[i].Length;
                }
            }
            return lengths;
        }
    }
}
