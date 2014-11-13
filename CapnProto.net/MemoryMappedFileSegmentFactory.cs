using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace CapnProto
{
    public unsafe sealed class MemoryMappedFileSegmentFactory : SegmentFactory
    {
        MemoryMappedFile file;
        MemoryMappedViewAccessor view;
        ulong* pointer;
        private long totalWords;

        public override void Dispose()
        {
            Cache<MemoryMappedFileSegmentFactory>.Push(this);
        }
        protected override void Reset(bool recycling)
        {
            pointer = (ulong*)0;
            if (view != null)
            {
                view.SafeMemoryMappedViewHandle.ReleasePointer();
                try { view.Dispose(); }
                catch { }
                view = null;
            }
            if (file != null)
            {
                file.Dispose();
                file = null;
            }
            base.Reset(recycling);
        }
        public static MemoryMappedFileSegmentFactory Open(string path,
            MemoryMappedFileAccess access = MemoryMappedFileAccess.Read, int defaultSegmentWords = DefaultSegmentWords)
        {
            long length = new FileInfo(path).Length;
            MemoryMappedFileSegmentFactory obj = null;
            try
            {
                obj = Cache<MemoryMappedFileSegmentFactory>.Pop() ?? new MemoryMappedFileSegmentFactory();
                obj.Init(path, 0, length >> 3, FileMode.Open, access, defaultSegmentWords);
                var tmp = obj;
                obj = null; // to avoid finally
                return tmp;
            }
            finally
            {
                Cache<MemoryMappedFileSegmentFactory>.Push(obj);
            }
        }

        private void Init(string path, long offsetBytes, long totalWords, FileMode fileMode, MemoryMappedFileAccess access, int defaultSegmentWords)
        {
            base.Init(defaultSegmentWords);
            FileAccess fileAccess;
            switch (access)
            {
                case MemoryMappedFileAccess.Read:
                case MemoryMappedFileAccess.ReadExecute:
                case MemoryMappedFileAccess.CopyOnWrite:
                    fileAccess = FileAccess.Read;
                    break;
                case MemoryMappedFileAccess.Write:
                    fileAccess = FileAccess.Write;
                    break;
                default:
                    fileAccess = FileAccess.ReadWrite;
                    break;
            }
            var fs = File.Open(path, fileMode, fileAccess);
            MemoryMappedFile file = null;
            MemoryMappedViewAccessor view = null;
            byte* ptr = (byte*)0;
            try
            {
                file = MemoryMappedFile.CreateFromFile(fs, null, 0, access, null, HandleInheritability.None, false);
                fs = null;

                view = file.CreateViewAccessor(offsetBytes, totalWords << 3, access);
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                this.pointer = (ulong*)&ptr[offsetBytes];
                this.view = view;
                this.file = file;
                view = null;
                file = null;
                this.totalWords = totalWords;
            }
            finally
            {
                if (fs != null) fs.Dispose();
                if (view != null)
                {
                    if (ptr != (byte*)0) view.SafeMemoryMappedViewHandle.ReleasePointer();
                    view.Dispose();
                }
                if (file != null) file.Dispose();
            }
        }
        protected override ISegment CreateEmptySegment()
        {
            return MemoryMappedFileSegment.Create();
        }
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            ((MemoryMappedFileSegment)segment).Init(&pointer[wordOffset], totalWords, activeWords);
            return true;
        }
        protected override bool TryReadWord(long wordOffset, out ulong value)
        {
            if (wordOffset < totalWords)
            {
                value = pointer[wordOffset];
                return true;
            }
            value = 0;
            return false;
        }

        private class MemoryMappedFileSegment : Segment
        {
            public static MemoryMappedFileSegment Create()
            {
                return Cache<MemoryMappedFileSegment>.Pop() ?? new MemoryMappedFileSegment();
            }
            public override void Dispose()
            {
                Cache<MemoryMappedFileSegment>.Push(this);
            }
            private ulong* pointer;
            internal void Init(ulong* pointer, int totalWords, int activeWords)
            {
                this.pointer = pointer;
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
                base.Reset(recycling);
            }
            public override ulong this[int index]
            {
                get { return pointer[index]; }
                set { pointer[index] = value; }
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

            public override void SetValue(int index, ulong value, ulong mask)
            {
                pointer[index] = (value & mask) | (pointer[index] & ~mask);
            }

            public override int WriteString(int index, string value, int bytes)
            {
                if (bytes-- > 0)
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
                if (bytes-- > 0)
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
                Marshal.Copy(new IntPtr(pointer + wordOffset), buffer, bufferOffset, wordsToCopy << 3);
                return wordsToCopy;
            }
            public override int WriteWords(int wordOffset, byte[] buffer, int bufferOffset, int maxWords)
            {
                int wordsToCopy = activeWords - wordOffset;
                if (wordsToCopy > maxWords) wordsToCopy = maxWords;
                Marshal.Copy(buffer, bufferOffset, new IntPtr(pointer + wordOffset), wordsToCopy << 3);
                return wordsToCopy;
            }
        }
    }
}
