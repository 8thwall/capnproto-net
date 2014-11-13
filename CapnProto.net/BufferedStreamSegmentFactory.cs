using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapnProto
{
    class BufferedStreamSegmentFactory : SegmentFactory
    {
        Stream source;
        long lastWord, remainingWords;
        readonly byte[] scratch = new byte[8];

        public static BufferedStreamSegmentFactory Create(Stream source, long length)
        {
            var obj = new BufferedStreamSegmentFactory();
            obj.Init(source, length);
            return obj;
        }

        private void Init(Stream source, long length)
        {
            this.source = source;
            this.lastWord = 0;
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
            return BufferSegment.Create();
        }

        protected override bool InitializeSegment(ISegment segment, long wordOffset, int totalWords, int activeWords)
        {
            CheckLastWord(wordOffset, totalWords);
            if(this.remainingWords >= 0 && this.remainingWords < totalWords)
            {
                return false;
            }
            byte[] buffer = new byte[totalWords << 3];
            if(!Read(source, buffer, buffer.Length))
            {
                return false;
            }
            ((BufferSegment)segment).Init(buffer, 0, totalWords, activeWords);
            if (this.remainingWords > 0) this.remainingWords-=totalWords;
            return true;
        }
        
    }
}
