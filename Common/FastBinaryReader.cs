using System.Buffers;
using System.IO;
using System.Text;

namespace FrugalCafe
{
    public class FastBinaryReader : BinaryReader
    {
        private readonly bool _utf8;
        private readonly bool _unicode;
        private byte[] _buffer;

        public FastBinaryReader(Stream input) : base(input)
        {
            _utf8 = true;
        }

        public FastBinaryReader(Stream input, Encoding encoding, bool leaveOpen = false) : base(input, encoding, leaveOpen)
        {
            _utf8 = encoding is UTF8Encoding;
            _unicode = encoding is UnicodeEncoding;
        }

        public override void Close()
        {
            if (_buffer != null)
            {
                ArrayPool<byte>.Shared.Return(_buffer);
            }

            base.Close();
        }

        public override unsafe string ReadString()
        {
            if (_utf8 || _unicode)
            {
                int len = base.Read7BitEncodedInt();

                if (len == 0)
                {
                    return string.Empty;
                }

                if ((_buffer == null) || (_buffer.Length < len))
                {
                    if (_buffer != null)
                    {
                        ArrayPool<byte>.Shared.Return(_buffer);
                    }

                    _buffer = ArrayPool<byte>.Shared.Rent(len);
                }

                int remain = len;
                len = 0;

                while (remain != 0)
                {
                    int read = base.BaseStream.Read(_buffer, len, remain);

                    if (read == 0)
                    {
                        throw new EndOfStreamException();
                    }

                    len += read;
                    remain -= read;
                }

                if (_utf8)
                {
                    return StringTable.Shared.InternUtf8(_buffer, len);
                }

                fixed (byte* p = _buffer)
                {
                    return StringTable.Shared.Intern((char *)p, len / 2);
                }
            }

            return base.ReadString();
        }
    }
}
