#if UNSAFE
#define UNMANAGED
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CapnProto
{
    class BufferedStreamSegmentFactory : SegmentFactory
    {
        Stream source;
        long lastWord, remainingWords;
        bool leaveOpen;
        readonly byte[] scratch = new byte[8];

        public static BufferedStreamSegmentFactory Create(Stream source, long length, bool leaveOpen)
        {
            var obj = Cache<BufferedStreamSegmentFactory>.Pop() ?? new BufferedStreamSegmentFactory();
            obj.Init(source, length, leaveOpen);
            return obj;
        }
        public override void Dispose()
        {
            Cache<BufferedStreamSegmentFactory>.Push(this);
        }
        protected override void Reset(bool recyclying)
        {
            if(source != null && !leaveOpen)
            {
                try { source.Dispose(); } catch { }
            }
            source = null;
            lastWord = remainingWords = 0;
            leaveOpen = false;
            base.Reset(recyclying);
        }
        private void Init(Stream source, long length, bool leaveOpen)
        {
            this.source = source;
            this.lastWord = 0;
            this.leaveOpen = leaveOpen;
            if (length < 0) remainingWords = -1;
            else remainingWords = length >> 3;
        }
        private void CheckLastWord(long wordOffset, int delta)
        {
            if (this.lastWord != wordOffset)
                throw new InvalidOperationException();
            this.lastWord += delta;
        }
        protected override bool TryReadWord(long wordOffset, out ulong value)
        {
            CheckLastWord(wordOffset, 1);
            if (this.remainingWords == 0)
            {
                value = 0;
                return false;
            }
            if(!Read(source, scratch, 8))
            {
                value = 0;
                return false;
            }
            
            if(this.remainingWords > 0) this.remainingWords--;
            value = BitConverter.ToUInt64(scratch, 0);
            return true;
        }
        static bool Read(Stream source, byte[] buffer, int count)
        {
            int offset = 0, read;
            while (count > 0 && (read = source.Read(buffer, offset, count)) > 0)
            {
                offset += read;
                count -= read;
            }
            return count == 0;
        }

        protected override ISegment CreateEmptySegment()
        {
#if UNMANAGED
            return PointerSegment.Create(true);
#else
            return BufferSegment.Create();
#endif
        }

        static byte[] sharedBuffer;
        private static byte[] PopBuffer()
        {
            return Interlocked.Exchange(ref sharedBuffer, null) ?? new byte[1024 * Message.WordLength];
        }
        private static void PushBuffer(byte[] buffer)
        {
            if(buffer != null) Interlocked.Exchange(ref sharedBuffer, buffer);
        }
        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            CheckLastWord(wordOffset, totalWords);
            
            if(this.remainingWords >= 0 && this.remainingWords < totalWords)
            {
                return false;
            }

#if UNMANAGED
            var ptr = default(IntPtr);
            byte[] buffer = null;
            try {
                long bytes = ((long)totalWords) << 3;
                ptr = Marshal.AllocHGlobal(totalWords << 3);
                buffer = PopBuffer();

                IntPtr writeHead = ptr;
                while(bytes > 0)
                {
                    int read = (int)Math.Min(bytes, buffer.Length);
                    if (!Read(source, buffer, read)) throw new EndOfStreamException();
                    Marshal.Copy(buffer, 0, writeHead, read);
                    writeHead += read;
                    bytes -= read;
                }
                ((PointerSegment)segment).Initialize(ptr, totalWords, activeWords);
                ptr = default(IntPtr);
            }
            finally
            {
                PushBuffer(buffer);
                if (ptr != default(IntPtr)) Marshal.FreeHGlobal(ptr);
            }
#else
            byte[] buffer = new byte[totalWords << 3];
            if (!Read(source, buffer, buffer.Length))
            {
                return false;
            }
            ((BufferSegment)segment).Init(buffer, 0, totalWords, activeWords);
#endif
            if (this.remainingWords > 0) this.remainingWords -= totalWords;
            return true;
        }
        
    }
}
