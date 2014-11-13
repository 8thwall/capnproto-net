using System.IO;
using System.IO.MemoryMappedFiles;

namespace CapnProto
{
    public sealed class MemoryMappedFileSegmentFactory : SegmentFactory
    {
        MemoryMappedFile file;
        MemoryMappedFileAccess access;
        private long totalWords, offset;

        public override void Dispose()
        {
            Cache<MemoryMappedFileSegmentFactory>.Push(this);
        }
        protected override void Reset(bool recycling)
        {
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
                obj.Init(path, 0, length >> 8, FileMode.Open, access, defaultSegmentWords);
                var tmp = obj;
                obj = null; // to avoid finally
                return tmp;
            }
            finally
            {
                Cache<MemoryMappedFileSegmentFactory>.Push(obj);
            }
        }

        private void Init(string path, long offset, long totalWords, FileMode fileMode, MemoryMappedFileAccess access, int defaultSegmentWords)
        {
            base.Init(defaultSegmentWords);
            this.access = access;
            FileAccess fileAccess;
            switch(access)
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
            try {
                file = MemoryMappedFile.CreateFromFile(fs, null, 0, access, null, HandleInheritability.None, false);
                fs = null;
                this.totalWords = totalWords;
                this.offset = offset;
            } finally {
                if(fs != null) fs.Dispose();
            }
        }
        protected override ISegment CreateEmptySegment()
        {
            return MemoryMappedFileSegment.Create();
        }
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            ((MemoryMappedFileSegment)segment).Init(file, wordOffset, totalWords, activeWords, access);
            return true;
        }
        protected override bool TryReadWord(long wordOffset, out ulong value)
        {
            if (wordOffset < totalWords)
            {
                using (var acc = file.CreateViewAccessor(this.offset + (wordOffset << 3), 8))
                {
                    value = acc.ReadUInt64(0);
                    return true;
                }
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
            internal void Init(MemoryMappedFile file, long wordOffset, int totalWords, int activeWords, MemoryMappedFileAccess access)
            {
                this.totalWords = totalWords;
                this.activeWords = activeWords;
                accessor = file.CreateViewAccessor(wordOffset << 3, totalWords << 3, access);
            }
            private int totalWords, activeWords;
            private MemoryMappedViewAccessor accessor;
            public override int Length
            {
                get { return activeWords; }
            }
            public override void Reset(bool recycling)
            {
                if (accessor != null) accessor.Dispose();
                accessor = null;
                base.Reset(recycling);                
            }
            public override ulong this[int index]
            {
                get { return accessor.ReadUInt64(index << 3); }
                set { accessor.Write(index << 3, value); }
            }
            protected override bool TryAllocate(int words, out int index)
            {
                int space = totalWords - activeWords;
                if(words <= space)
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
                accessor.Write(index << 3, (value & mask) | (accessor.ReadUInt64(index << 3) & ~mask));
            }
        }
    }
}
