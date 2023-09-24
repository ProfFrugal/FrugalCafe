using System;
using System.Text;
using System.IO;
using System.Runtime.CompilerServices;
using System.Buffers;

namespace FrugalCafe
{
    public class SimpleStreamWriter : IDisposable
    {
        const int CharBufferSize = 32 * 1024;

        private char[] _charBuffer;
        private byte[] _byteBuffer;
        
        private int _pos;
        private bool _leaveOpen;
        private Encoding _encoding;

        Stream _output;

        public SimpleStreamWriter(Stream output, Encoding encoding, bool leaveOpen = false)
        {
            _output = output;
            _encoding = encoding ?? Encoding.UTF8;
            _charBuffer = ArrayPool<char>.Shared.Rent(CharBufferSize);
            _byteBuffer = ArrayPool<byte>.Shared.Rent(_encoding.GetMaxByteCount(CharBufferSize));
            _leaveOpen = leaveOpen;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value)
        {
            if (value != null)
            {
                Write(value, 0, value.Length);
            }
        }

        public void Write(string value, int start, int length)
        {
            while (length > 0)
            {
                if (_pos == CharBufferSize)
                {
                    Flush();
                }

                int len = Math.Min(length, CharBufferSize - _pos);

                if (len == 1)
                {
                    _charBuffer[_pos] = value[start];
                }
                else if (len == 2)
                {
                    _charBuffer[_pos] = value[start];
                    _charBuffer[_pos + 1] = value[start + 1];
                }
                else
                {
                    value.CopyTo(start, _charBuffer, _pos, len);
                }

                start += len;
                length -= len;
                _pos += len;
            }
        }

        public void Write(char[] value, int start, int length)
        {
            while (length > 0)
            {
                if (_pos == CharBufferSize)
                {
                    Flush();
                }

                int len = Math.Min(length, CharBufferSize - _pos);

                if (len == 1)
                {
                    _charBuffer[_pos] = value[start];
                }
                else if (len == 2)
                {
                    _charBuffer[_pos] = value[start];
                    _charBuffer[_pos + 1] = value[start + 1];
                }
                else
                {
                    Buffer.BlockCopy(value, start, _charBuffer, _pos, len * sizeof(char));
                }

                start += len;
                length -= len;
                _pos += len;
            }
        }

        public void WriteLine()
        {
            WriteChar('\r');
            WriteChar('\n');
        }

        public void Close()
        {
            Flush();

            if (!_leaveOpen)
            {
                _output.Close();
            }

            _output = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteChar(char ch)
        {
            if (_pos == CharBufferSize)
            {
                Flush();
            }

            _charBuffer[_pos++] = ch;
        }

        public void Write(long value)
        {
            if (value < 0)
            {
                WriteChar('-');

                Write((ulong)(-value));
            }
            else
            {
                Write((ulong)(value));
            }
        }

        public unsafe void Write(ulong value)
        {
            char* buffer = stackalloc char[20];

            int len = 0;

            do
            {
                buffer[len++] = (char)('0' + value % 10);
                value /= 10;
            }
            while (value != 0);

            for (int i = len - 1; i >= 0; i--)
            {
                WriteChar(buffer[i]);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Flush()
        {
            if (_pos != 0)
            {
                int len = _encoding.GetBytes(_charBuffer, 0, _pos, _byteBuffer, 0);

                _output.Write(_byteBuffer, 0, len);
                _pos = 0;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }

            ArrayPool<char>.Shared.Return(_charBuffer);
            ArrayPool<byte>.Shared.Return(_byteBuffer);
        }
    }
}
