using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace CapnProto
{
    public interface IRecyclable
    {
        void Reset(bool recycling);
    }
    internal static class Cache<T> where T : class, IRecyclable
    {
        [ThreadStatic]
        private static T recycled;

        public static T Pop()
        {
            var tmp = recycled;
            if (tmp != null)
            {
                recycled = null;
                GC.ReRegisterForFinalize(tmp);
                return tmp;
            }
            return null;
        }

        public static void Push(T obj)
        {
            if (obj != null)
            {
                // note: don't want to add GC.SuppressFinalize
                // to Reset, in case Reset is called independently
                // of lifetime management
                if (recycled == null)
                {
                    obj.Reset(true);
                    GC.SuppressFinalize(obj);
                    recycled = obj;
                }
                else
                {
                    obj.Reset(false);
                    GC.SuppressFinalize(obj);
                }
            }
        }
    }
    abstract public partial class CapnProtoReader : IDisposable, IRecyclable
    {

        
        public override string ToString()
        {
            return GetType().Name;
        }
        protected virtual void OnInitialize()
        {
            OnChangeSegment(0, firstSegment);
        }
        void IRecyclable.Reset(bool recycling) { Reset(recycling); }
        protected virtual void Reset(bool recycling)
        {
            context = null;
            if(recycling)
            {
                segmentCount = 0;
                underlyingBaseOffset = 0;
            }
            else
            {
                otherSegments = null;
            }
        }
        protected virtual void OnDispose(bool disposing)
        {
            if (disposing)
            {
                Reset(false);
            }
        }
        protected void Init(object context)
        {
            this.context = context;
        }
        public void Crawl(TextWriter output)
        {
            int count = SegmentCount;
            if(count == 0)
            {
                ReadSegmentsHeader();
                count = SegmentCount;
            }
            output.WriteLine("segments: {0}", count);
            for(int i = 0;i < count; i++)
            {
                var range = GetSegment(i);
                output.WriteLine("segment {0}: {1} words, offset: {2}", i, range.Length, range.Offset);
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

                output.WriteLine("{0:00}/{1:000000}: {2}", next.Segment, next.EffectiveOffset, PointerType.GetName(next.Pointer));

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
                            case ElementSize.InlineComposite:
                                var tagWord = ReadWord(next.Segment, next.EffectiveOffset);
                                int data = unchecked((int)(ushort)(tagWord >> 32)),
                                    pointers = unchecked((int)(ushort)(tagWord >> 48)),
                                    itemCount = unchecked((int)(((uint)tagWord) >> 2)),
                                    itemSize = data + pointers;
                                ulong itemPointer = tagWord & 0xFFFFFFFF00000003;
                                output.WriteLine("\tdata: {0}; pointers: {1}; items: {2} (each {3} words)", data, pointers, itemCount, itemSize);
                                if (pointers != 0 && itemCount != 0)
                                {
                                    int offset = next.EffectiveOffset + 1;
                                    string itemTypeName = PointerType.GetName(itemPointer); // the same for all elements
                                    for (int i = 0; i < itemCount; i++)
                                    {
                                        output.WriteLine("\t{0:00}/{1:000000}: element {2} - {3}", next.Segment, offset, i, itemTypeName);
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
                            var innerPtr = ReadWord(next.Segment, next.EffectiveOffset);
                            var tagWord = ReadWord(next.Segment, next.EffectiveOffset + 1);

                            var newPointer = new PendingPointer(next.Segment, next.EffectiveOffset, next.EffectiveOffset, innerPtr, tagWord);
                            output.WriteLine("\tdouble-far resolved to {0:00}/{1:000000}", newPointer.Segment, newPointer.EffectiveOffset);
                            pending.Add(newPointer, null);                            
                        }
                        else
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
            else if((ptr & PointerType.Mask) == PointerType.Other)
            {
                output.WriteLine("\t\tSkipping pointer: " + PointerType.GetName(ptr));
            }
            else
            {
                var newPtr = new PendingPointer(parent.Segment, parent.EffectiveOffset, offset + 1, ptr);
                output.WriteLine("\t\t{0}: {1} at {2:00}/{3:000000}", index, PointerType.GetName(ptr), newPtr.Segment, newPtr.EffectiveOffset);
                if (pending.ContainsKey(newPtr))
                {
                    output.WriteLine("\t\t\tduplicated key");
                    foreach (PendingPointer key in pending.Keys)
                    {
                        if(key.Equals(newPtr))
                        {
                            output.WriteLine("\t\t\talso referenced from {0:00}/{1:000000}", key.ParentSegment, key.ParentOffset);
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


        //public static CapnProtoReader Create(string path, object context = null, long offset = 0, long count = -1)
        //{
        //    return CapnProtoMemoryMappedFileReader.Create(path, context, offset, count);
        //}
        public static CapnProtoReader Create(byte[] source, object context = null, int offset = 0, int count = -1)
        {
            return CapnProtoBlobReader.Create(source, offset, count, context);
        }
        public static CapnProtoReader Create(Stream source, object context = null, bool leaveOpen = false)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (source is MemoryStream)
            {
                var ms = (MemoryStream)source;
                // unfortunately, we can't access ms._origin, so we can't be sure what
                // region the caller wants to use - unless we can prove that they want
                // all of it
                byte[] buffer;
                try
                {
                    buffer = ms.GetBuffer();
                } catch(UnauthorizedAccessException)
                {
                    // not all buffers are available to the caller
                    buffer = null;
                }
                if(buffer != null && buffer.Length == (int)ms.Length)
                    return Create(buffer, context, 0, buffer.Length);
            }
            return CapnProtoStreamReader.Create(source, context, leaveOpen);
        }

        protected CapnProtoReader() { }

        protected static readonly byte[] NilBytes = new byte[0];
        private static readonly bool isLittleEndian = BitConverter.IsLittleEndian;
        public void Dispose()
        {
            OnDispose(true);
        }

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
                TypeModel.Log("{0}: de-referencing from {1:00}/{2:000000}", PointerType.GetName(pointer), segment, origin);
                pointer = ResolveFarPointer(pointer, ref segment, ref origin);
                TypeModel.Log("{0}: de-referenced to {1:00}/{2:000000}", PointerType.GetName(pointer), segment, origin);
            }
            int count = ListPointer.Parse(pointer, ref origin, out size);
            if (count == 0) return new List<T>();
            TypeModel.Log("{0}: Deserializing {1} list of {2} from {3:00}/{4:000000} (size {5})",
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
                            TypeModel.Log("{0}: element {1} of {2} at {3:00}/{4:000000}", context.Depth, i, count, segment, origin);
                            list.Add(model.Deserialize<T>(segment, origin + i, context, raw[i]));
                        }
                        return list;
                    }
                case ElementSize.InlineComposite:
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
                            TypeModel.Log("{0}: element {1} of {2} at {3:00}/{4:000000}", context.Depth, i, numberOfElements, segment, origin);
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
        internal ulong ResolveFarPointer(ulong pointer, ref int segment, ref int origin)
        {
            int newSegment, newOrigin;
            bool doubleLandingPad = DereferenceFarPointer(pointer, out newSegment, out newOrigin);
            TypeModel.Log("far-pointer resolved to landing-pad at {0:00}/{1:000000}", newSegment, newOrigin);
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
                TypeModel.Log("far-pointer resolved to landing-pad at {0:00}/{1:000000}", newSegment, newOrigin);
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

        protected abstract void OnChangeSegment(int index, SegmentRange range);

        private int segment, segmentCount;
        public int Segment { get { return segment; } }

        public int SegmentCount
        {
            get { return segmentCount; }
        }
        public void ChangeSegment(int segment)
        {
            if (segment != this.segment)
            {
                if (segment == 0)
                {
                    OnChangeSegment(0, firstSegment);
                }
                else
                {

                    if (segment < 0 || segment > otherSegments.Length)
                        throw new ArgumentOutOfRangeException("segment");

                    OnChangeSegment(segment, otherSegments[segment - 1]);
                }
                this.segment = segment;
            }
        }
        public byte[] ReadBytesFromPointer(int segment, int wordOffset, ulong ptr)
        {
            if ((ptr & PointerType.Mask) == PointerType.Far)
            {
                ptr = ResolveFarPointer(ptr, ref segment, ref wordOffset);
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
                ptr = ResolveFarPointer(ptr, ref segment, ref wordOffset);
            }
            var lptr = new ListPointer(ptr);
            if (lptr.ElementSize != ElementSize.OneByte) throw new InvalidOperationException("Expected pointer to single-byte data");

            int len = lptr.Size;
            if (len == 0) throw ExpectedNullTerminatedString();

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

        protected virtual string ReadString(int segment, int wordOffset, int count)
        {
            if (count <= ScratchLengthBytes)
            {
                var tmp = Scratch;
                ReadWords(segment, wordOffset, tmp, 0, ToWords(count));
                if (tmp[count - 1] != 0) throw ExpectedNullTerminatedString();

                return Encoding.UTF8.GetString(tmp, 0, count - 1);
            }
            // this is a terrible implementation
            var bytes = ReadBytes(segment, wordOffset, count);
            if (bytes[count - 1] != 0) throw ExpectedNullTerminatedString();
            return Encoding.UTF8.GetString(bytes, 0, count - 1);
        }
        protected abstract byte[] ReadBytes(int segment, int wordOffset, int count);

        private SegmentRange firstSegment;
        private SegmentRange[] otherSegments;

        public void SetSegments(SegmentRange range, long baseOffset = 0)
        {
            firstSegment = range;
            otherSegments = null;
            underlyingBaseOffset = baseOffset;
            segmentCount = 1;
            OnInitialize();
        }
        public void SetSegments(SegmentRange[] ranges, long baseOffset = 0)
        {
            if (ranges == null || ranges.Length == 0)
            {
                firstSegment = default(SegmentRange);
                otherSegments = null;
                segmentCount = 0;
            } else
            {
                firstSegment = ranges[0];
                if(ranges.Length == 1)
                {
                    otherSegments = null;
                }
                else
                {
                    Array.Resize(ref otherSegments, ranges.Length - 1);
                    Array.Copy(ranges, 1, otherSegments, 0, ranges.Length - 1);
                }
                segmentCount = ranges.Length;
            }
            underlyingBaseOffset = baseOffset;
            OnInitialize();
        }
        private long underlyingBaseOffset;
        public long UnderlyingBaseOffset { get { return underlyingBaseOffset; } }

        protected abstract bool TryReadWordDirect(long byteOffset, out ulong word);
        private ulong ReadWordDirect(long byteOffset)
        {
            ulong word;
            if(TryReadWordDirect(byteOffset, out word)) return word;
            throw new EndOfStreamException();
        }
        public bool ReadSegmentsHeader()
        {
            long baseOffset;
            int currentSegments = SegmentCount;
            if(currentSegments == 0)
            {
                baseOffset = underlyingBaseOffset;
            } else
            {
                var last = GetSegment(currentSegments - 1);
                baseOffset = underlyingBaseOffset + 8 * (last.Offset + last.Length);
            }

            ulong word;
            if (!TryReadWordDirect(baseOffset, out word))
            {
                return false;
            }
            baseOffset += 8;
            int additionalSegments = (int)word;

            var firstSegment = new SegmentRange(0, (int)(word >> 32));
            var otherSegments = additionalSegments == 0 ? null : new SegmentRange[additionalSegments];
            int segmentOffset = firstSegment.Length;
            if (segmentOffset == 0)
                throw new InvalidOperationException("First segment cannot be empty");

            if (additionalSegments != 0)
            {
                int index = 0;
                for (int i = 0; i < additionalSegments / 2; i++)
                {
                    word = ReadWordDirect(baseOffset);
                    baseOffset += 8;
                    var range = new SegmentRange(segmentOffset, (int)word);
                    otherSegments[index++] = range;
                    segmentOffset += range.Length;
                    range = new SegmentRange(segmentOffset, (int)(word >> 32));
                    otherSegments[index++] = range;
                    segmentOffset += range.Length;
                }

                if ((additionalSegments % 2) != 0)
                {
                    word = ReadWordDirect(baseOffset);
                    baseOffset += 8;
                    var range = new SegmentRange(segmentOffset, (int)word);
                    otherSegments[index] = range;
                    segmentOffset += range.Length; // don't really need this, since last segment!
                }
            }

            // all done successfully: update the fields
            this.segmentCount = additionalSegments + 1;
            this.firstSegment = firstSegment;
            this.otherSegments = otherSegments;
            this.underlyingBaseOffset = baseOffset;

            // and move to the data
            OnInitialize();
            return true;
        }
        public struct SegmentRange
        {
            public readonly int Offset, Length;
            public SegmentRange(int offset, int length)
            {
                this.Offset = offset;
                this.Length = length;
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
            if ((pointer & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Expected struct pointer; got " + PointerType.GetName(pointer));
            var offset = unchecked(((int)pointer) >> 2); // note: int because signed
            var count = (int)((pointer >> 32) & 0xFFFF);
            TypeModel.Log("reading data payload from {0:00}/{1:000000}: {2} words", segment, origin + offset, Math.Max(count, max));
            ReadWords(segment, origin + offset, count, max, raw);
        }

        public unsafe int ReadPointers(int segment, int origin, ulong pointer, ulong* raw, int max)
        {
            if ((pointer & PointerType.Mask) != PointerType.Struct) throw new InvalidOperationException("Expected struct pointer; got " + PointerType.GetName(pointer));
            var offset = unchecked(((int)pointer) >> 2); // note: int because signed
            var dataCount = (int)((pointer >> 32) & 0xFFFF);
            var count = (int)((pointer >> 48) & 0xFFFF);

            int pointerBase = origin + offset + dataCount;
            TypeModel.Log("reading pointers from {0:00}/{1:000000}: {2} words", segment, pointerBase, Math.Max(count, max));
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

        public SegmentRange GetSegment(int index)
        {
            if (index >= 0 && index < segmentCount)
            {
                if (index == 0) return firstSegment;

                if (index > 0 && otherSegments != null && index <= otherSegments.Length)
                    return otherSegments[index - 1];
            }
            throw new ArgumentOutOfRangeException("index");            
        }
    }
}
