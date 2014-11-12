using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace CapnProto
{
    internal class MemoryMappedFileSegmentFactory : SegmentFactory, IRecyclable
    {
        MemoryMappedFile file;
        private bool @readonly;
        private long totalWords, offset;
        void IRecyclable.Reset(bool recycling)
        {
            if (file != null)
            {
                file.Dispose();
                file = null;
            }
        }
        public static MemoryMappedFileSegmentFactory Load(string path, bool @readonly = false)
        {
            long length = new FileInfo(path).Length;
            MemoryMappedFileSegmentFactory obj = null;
            try
            {
                obj = Cache<MemoryMappedFileSegmentFactory>.Pop() ?? new MemoryMappedFileSegmentFactory();
                obj.Init(path, 0, length >> 8, @readonly);
                var tmp = obj;
                obj = null; // to avoid finally
                return tmp;
            }
            finally
            {
                Cache<MemoryMappedFileSegmentFactory>.Push(obj);
            }
        }

        private void Init(string path, long offset, long totalWords, bool @readonly)
        {
            this.@readonly = @readonly;
            file = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0, Access);
            this.totalWords = totalWords;
            this.offset = offset;
        }
        private MemoryMappedFileAccess Access { get { return @readonly ? MemoryMappedFileAccess.Read : MemoryMappedFileAccess.ReadWrite; } }

        public override void Dispose()
        {
            Cache<MemoryMappedFileSegmentFactory>.Push(this);
        }
        protected override ISegment CreateEmptySegment()
        {
            throw new System.NotImplementedException();
        }
        protected override void InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            throw new System.NotImplementedException();
        }
        protected override bool TryReadUInt64(long offset, out ulong value)
        {
            if (offset < totalWords)
            {
                using (var acc = file.CreateViewAccessor(this.offset + (offset << 3), 8))
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
            private int maxWords, activeWords;
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
                get
                {
                    return accessor.ReadUInt64(index << 3);
                }
                set
                {
                    accessor.Write(index << 3, value);
                }
            }
            public override bool TryAllocate(int words, out int index)
            {
                int space = maxWords - activeWords;
                if(words <= space)
                {
                    index = activeWords;
                    activeWords += words;
                    return true;
                }
                index = 0;
                return false;
            }
        }
    }
}
