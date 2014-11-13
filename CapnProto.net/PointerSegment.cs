using System;
using System.Runtime.InteropServices;
using System.Text;

namespace CapnProto
{
    
    public unsafe class PointerSegment : Segment
    {
        private class PointerOwningSegment : PointerSegment
        {
            public override void Reset(bool recycling)
            {
                FreePointer();
                base.Reset(recycling);
            }
            public override void Dispose()
            {
                Cache<PointerOwningSegment>.Push(this);
            }
            ~PointerOwningSegment()
            {
                FreePointer();
            }
        }
        public static PointerSegment Create(bool free)
        {
            if (free) return Cache<PointerOwningSegment>.Pop() ?? new PointerOwningSegment();
            return Cache<PointerSegment>.Pop() ?? new PointerSegment();
        }
        protected void FreePointer()
        {
            if (pointer != (ulong*)0) Marshal.FreeHGlobal(new IntPtr(pointer));
            pointer = (ulong*)0;
        }
        public override void Dispose()
        {
            Cache<PointerSegment>.Push(this);
        }
        private ulong* pointer;
        public void Initialize(IntPtr pointer, int totalWords, int activeWords)
        {
            if (totalWords <= 0) throw new InvalidOperationException("A segment cannot be empty");
            this.pointer = (ulong*)pointer;
            this.totalWords = totalWords;
            this.activeWords = activeWords;
        }
        private int totalWords, activeWords;

        public override int Length
        {
            get { return activeWords; }
        }
        public override void Reset(bool recycling)
        {
            pointer = (ulong*)0;
            activeWords = totalWords = 0;
            base.Reset(recycling);
        }

        public override ulong this[int index]
        {
            get {
                if ((index & MSB32) == 0 && index < activeWords) return pointer[index];
                throw new IndexOutOfRangeException();
            }
            set {
                if ((index & MSB32) == 0 && index < activeWords)
                {
                    pointer[index] = value;
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
        protected override bool TryAllocate(int words, out int index)
        {
            int space = totalWords - activeWords;
            if (words <= space)
            {
                index = activeWords;
                activeWords += words;
                return true;
            }
            index = 0;
            return false;
        }

        private const int MSB32 = 1 << 31;
        
        public override void SetValue(int index, ulong value, ulong mask)
        {
            if ((index & MSB32) == 0 && index < activeWords)
            {
                pointer[index] = (value & mask) | (pointer[index] & ~mask);
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        public override int WriteString(int index, string value, int bytes)
        {
            if ((index & MSB32) == 0 && bytes-- > 0 && (index + (bytes >> 3)) < activeWords)
            {
                byte* ptr = (byte*)&pointer[index];
                fixed (char* chars = value)
                {
                    return Encoding.GetBytes(chars, value.Length, ptr, bytes);
                }
            }
            throw new InvalidOperationException();
        }

        public override string ReadString(int index, int bytes)
        {
            if ((index & MSB32) == 0 && bytes-- > 0 && (index + (bytes >> 3)) < activeWords)
            {
                byte* ptr = (byte*)&pointer[index];
                if (ptr[bytes] == 0)
                {
                    Decoder dec = null;
                    int chars;
                    try
                    {
                        dec = PopDecoder();
                        chars = dec.GetCharCount(ptr, bytes, true);
                    }
                    finally
                    {
                        PushDecoder(dec);
                    }
                    return new string((sbyte*)ptr, 0, chars, Encoding);
                }
            }
            throw new InvalidOperationException();
        }

        public override int ReadWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int wordsToCopy = activeWords - wordOffset;
            if (wordsToCopy > maxWords) wordsToCopy = maxWords;
            if (wordsToCopy > 0)
            {
                Marshal.Copy(new IntPtr(pointer + wordOffset), buffer, bufferOffset, wordsToCopy << 3);
            }
            return wordsToCopy;
        }
        public override int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
        {
            int wordsToCopy = activeWords - wordOffset;
            if (wordsToCopy > maxWords) wordsToCopy = maxWords;
            if (wordsToCopy > 0)
            {
                Marshal.Copy(buffer, bufferOffset, new IntPtr(pointer + wordOffset), wordsToCopy << 3);
            }
            return wordsToCopy;
        }
    }
}
