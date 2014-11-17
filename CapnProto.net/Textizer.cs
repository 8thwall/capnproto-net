using System;
using System.IO;
using System.Text;

namespace CapnProto
{
    class Textizer : IRecyclable, IDisposable
    {
        static Textizer Create()
        {
            return Cache<Textizer>.Pop() ?? new Textizer();
        }
        void IDisposable.Dispose()
        {
            Cache<Textizer>.Push(this);
        }
        private static readonly Encoding encoding = new UTF8Encoding(false);

        void IRecyclable.Reset(bool reusing)
        {
            if (reusing)
            {
#if !PCL
                if (encoder != null) encoder.Reset();
#endif
                if (decoder != null) decoder.Reset();
            }
        }

        private readonly char[] chars;
        private readonly byte[] bytes;
        private Encoder encoder;
        private Encoder Encoder { get{return encoder ?? (encoder = encoding.GetEncoder());}}
        private Decoder decoder;
        private Decoder Decoder { get { return decoder ?? (decoder = encoding.GetDecoder()); } }
        public Textizer()
        {
            chars = new char[CHAR_LENGTH];
            bytes = new byte[BYTE_LENGTH];
        }
        const int CHAR_LENGTH = 512, MAX_BYTES_TO_DECODE = CHAR_LENGTH, MAX_CHARS_TO_ENCODE = CHAR_LENGTH;
        static readonly int BYTE_LENGTH = encoding.GetMaxByteCount(CHAR_LENGTH) + 8; // leave space for the leftovers


        public static int AppendTo(Pointer pointer, TextWriter destination)
        {
            pointer = pointer.Dereference();
            if (!pointer.IsValid) return 0;
            int len = pointer.SingleByteLength;
            if (--len <= 0 || pointer.GetListByte(len) != 0) return 0;
            using (var text = Create())
            {
                var decoder = text.Decoder;
                byte[] bytes = text.bytes;
                char[] chars = text.chars;
                int wordOffset = 0, totalChars = 0;
                do
                {
                    int bytesThisPass = Math.Min(len, MAX_BYTES_TO_DECODE);
                    int wordsThisPass = ((bytesThisPass - 1) >> 3) + 1;
                    pointer.ReadWords(wordOffset, bytes, 0, wordsThisPass);
                    int charCount = decoder.GetChars(bytes, 0, bytesThisPass, chars, 0, false);
                    totalChars += charCount;
                    wordOffset += wordsThisPass;
                    len -= bytesThisPass;
                    if (charCount != 0) destination.Write(chars, 0, charCount);
                } while (len > 0);
                return totalChars;
            }
        }
        public static int Count(Pointer pointer)
        {
            pointer = pointer.Dereference();
            if (!pointer.IsValid) return 0;
            int len = pointer.SingleByteLength;
            if (--len <= 0 || pointer.GetListByte(len) != 0) return 0;
            using (var text = Create())
            {
                var decoder = text.Decoder;
                byte[] bytes = text.bytes;
                int wordOffset = 0, totalChars = 0;
                do
                {
                    int bytesThisPass = Math.Min(len, MAX_BYTES_TO_DECODE);
                    int wordsThisPass = ((bytesThisPass - 1) >> 3) + 1;
                    pointer.ReadWords(wordOffset, bytes, 0, wordsThisPass);
                    int charCount = decoder.GetCharCount(bytes, 0, bytesThisPass, false);
                    totalChars += charCount;
                    wordOffset += wordsThisPass;
                    len -= bytesThisPass;
                } while (len > 0);
                return totalChars;
            }
        }
        public static int AppendTo(Pointer pointer, StringBuilder destination)
        {
            pointer = pointer.Dereference();
            if (!pointer.IsValid) return 0;
            int len = pointer.SingleByteLength;
            if (--len <= 0 || pointer.GetListByte(len) != 0) return 0;
            using (var text = Create())
            {
                var decoder = text.Decoder;
                byte[] bytes = text.bytes;
                char[] chars = text.chars;
                int wordOffset = 0, totalChars = 0;
                do
                {
                    int bytesThisPass = Math.Min(len, MAX_BYTES_TO_DECODE);
                    int wordsThisPass = ((bytesThisPass - 1) >> 3) + 1;
                    pointer.ReadWords(wordOffset, bytes, 0, wordsThisPass);
                    int charCount = decoder.GetChars(bytes, 0, bytesThisPass, chars, 0, false);
                    totalChars += charCount;
                    wordOffset += wordsThisPass;
                    len -= bytesThisPass;
                    if (charCount != 0) destination.Append(chars, 0, charCount);
                } while (len > 0);
                return totalChars;
            }
        }

        internal static void Write(Pointer pointer, char[] value, int offset, int count)
        {
            pointer = pointer.Dereference();
            if (!pointer.IsValid) throw new InvalidOperationException();
            int len = pointer.SingleByteLength;
            if(--len == 0) throw new InvalidOperationException();
            if (len == 0)
            {   // empty string
                pointer.SetListByte(len, 0);
                return;
            }

            
            using (var text = Create())
            {
                var encoder = text.Encoder;                    
                byte[] bytes = text.bytes;
                int byteOffset = 0, wordOffset = 0, totalBytes = 0;
                do
                {
                    int charsThisPass = Math.Min(count, MAX_CHARS_TO_ENCODE);
                    int newBytes = encoder.GetBytes(value, offset, charsThisPass, bytes, byteOffset, false);

                    // note: we could have some complete words, and some partial words; need to be careful

                    int availableBytes = newBytes + byteOffset;
                    int fullWords = availableBytes >> 3;
                    if(fullWords != 0) pointer.WriteWords(wordOffset, bytes, 0, fullWords);
                    totalBytes += newBytes;
                    wordOffset += fullWords;
                    byteOffset = availableBytes & 7;
                    if(byteOffset != 0)
                    {
                        // copy down those spare bytes
                        Buffer.BlockCopy(bytes, fullWords << 3, bytes, 0, byteOffset);
                    }
                    offset += charsThisPass;
                    count -= charsThisPass;
                } while (count > 0);

                // copy in any spare trailing bytes
                int origin = wordOffset << 3;
                for (int i = 0; i < byteOffset;i++ )
                {
                    pointer.SetListByte(origin++, bytes[i]);
                }
                pointer.SetListByte(len, 0);

                if (totalBytes != len) throw new InvalidOperationException("String encoded length mismatch");
            }
            
        }

#if FULLCLR
        internal static Text Create(Pointer pointer, System.Data.IDataRecord reader, int fieldIndex)
        {
            // TODO: replace with GetChars work

            return Text.Create(pointer, reader.GetString(fieldIndex));
            //using(var text = Create())
            //{
            //    var bytes = text.bytes;
            //    var chars = text.chars;
            //    var encoder = text.Encoder;

            //    int fieldOffset = 0, charsRead, byteOffset = 0;
            //    do
            //    {
            //        charsRead = (int)reader.GetChars(fieldIndex, fieldOffset, chars, 0, MAX_CHARS_TO_ENCODE);
            //        int newBytes = encoder.GetBytes(chars, 0, charsRead, bytes, byteOffset, false);

            //        int availableBytes = newBytes + byteOffset;
            //        int fullWords = availableBytes >> 3;

                    
            //    } while (charsRead != 0);

            //    throw new NotImplementedException();
            //}
        }
#endif
    }

}
