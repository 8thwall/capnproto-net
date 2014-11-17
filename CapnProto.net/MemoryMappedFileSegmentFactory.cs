#if UNSAFE

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;


namespace CapnProto
{
    public unsafe sealed class MemoryMappedFileSegmentFactory : SegmentFactory
    {
        MemoryMappedFile mmFile;
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
            if (mmFile != null)
            {
                mmFile.Dispose();
                mmFile = null;
            }
            base.Reset(recycling);
        }
        public static MemoryMappedFileSegmentFactory Open(string path, long offset = 0, long length = -1,
            MemoryMappedFileAccess access = MemoryMappedFileAccess.Read, int defaultSegmentWords = DefaultSegmentWords)
        {
            MemoryMappedFileSegmentFactory obj = null;
            try
            {
                obj = Cache<MemoryMappedFileSegmentFactory>.Pop() ?? new MemoryMappedFileSegmentFactory();
                obj.Init(path, 0, length, FileMode.Open, access, defaultSegmentWords);
                var tmp = obj;
                obj = null; // to avoid finally
                return tmp;
            }
            finally
            {
                Cache<MemoryMappedFileSegmentFactory>.Push(obj);
            }
        }

        public static MemoryMappedFileSegmentFactory Open(MemoryMappedFile file, long offset = 0, long length = -1,
            MemoryMappedFileAccess access = MemoryMappedFileAccess.Read, int defaultSegmentWords = DefaultSegmentWords, bool leaveOpen = false)
        {
            MemoryMappedFileSegmentFactory obj = null;
            try
            {
                obj = Cache<MemoryMappedFileSegmentFactory>.Pop() ?? new MemoryMappedFileSegmentFactory();
                obj.Init(file, offset, length, access, defaultSegmentWords, leaveOpen);
                var tmp = obj;
                obj = null; // to avoid finally
                return tmp;
            }
            finally
            {
                Cache<MemoryMappedFileSegmentFactory>.Push(obj);
            }
        }

        private void Init(MemoryMappedFile file, long offsetBytes, long lengthBytes, MemoryMappedFileAccess access, int defaultSegmentWords, bool leaveOpen = false)
        {
            base.Init(defaultSegmentWords);

            MemoryMappedViewAccessor view = null;
            byte* ptr = (byte*)0;
            try
            {
                if(lengthBytes < 0) lengthBytes = 0;
                view = file.CreateViewAccessor(offsetBytes, lengthBytes, access);
                lengthBytes = view.Capacity;
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
                this.pointer = (ulong*)&ptr[offsetBytes];
                this.view = view;
                if (!leaveOpen) this.mmFile = file;
                view = null;
                this.totalWords = lengthBytes >> 3;
            }
            finally
            {
                if (view != null)
                {
                    if (ptr != (byte*)0) view.SafeMemoryMappedViewHandle.ReleasePointer();
                    view.Dispose();
                }
                
            }
        }

        private void Init(string path, long offsetBytes, long lengthBytes, FileMode fileMode, MemoryMappedFileAccess access, int defaultSegmentWords)
        {
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
            FileStream fs = null;
            MemoryMappedFile file = null;
            try
            {
                fs = File.Open(path, fileMode, fileAccess);
                file = MemoryMappedFile.CreateFromFile(fs, null, 0, access, null, HandleInheritability.None, false);
                Init(file, offsetBytes, lengthBytes, access, defaultSegmentWords, false);
                fs = null;
                file = null;
            }
            finally
            {
                if (fs != null) fs.Dispose();
                if (file != null) file.Dispose();
                if (fs != null) fs.Dispose();
            }
            
        }
        protected override ISegment CreateEmptySegment()
        {
            return PointerSegment.Create(false);
        }
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            ((PointerSegment)segment).Initialize(new IntPtr(&pointer[wordOffset]), totalWords, activeWords);
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
    }
}
#endif