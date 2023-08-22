using Microsoft.Extensions.ObjectPool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace FrugalCafe
{
    public static class StringBuilderExtensions
    {
        public const string HexDigits = "0123456789abcdef";

        public const int MaximumCharCountNoLoh = (85000 - 64) / 2;

        public readonly static ObjectPool<StringBuilder> BuilderPool = 
            (new DefaultObjectPoolProvider()).CreateStringBuilderPool(
                initialCapacity:256, 
                maximumRetainedCapacity: StringBuilderExtensions.MaximumCharCountNoLoh);

        private static char[] reusedBuffer;

        public static StringBuilder AcquireBuilder()
        {
            return BuilderPool.Get();
        }

        public static void Release(this StringBuilder builder)
        {
            if (builder != null)
            {
                BuilderPool.Return(builder);
            }
        }

        public static string ToStringAndRelease(this StringBuilder builder) 
        { 
            if (builder == null)
            {
                return null;
            }

            string result = builder.ToString();

            BuilderPool.Return(builder);

            return result;
        }

        public static StringBuilder AppendNoLoh(this StringBuilder builder, string text)
        {
            if (text != null)
            {
                builder.AppendNoLoh(text, 0, text.Length);
            }

            return builder;
        }

        public static StringBuilder AppendNoLoh(this StringBuilder builder, string text, int start, int length)
        {
            while (length > 0)
            {
                int len = Math.Min(StringBuilderExtensions.MaximumCharCountNoLoh, length);

                builder.Append(text, start, len);
                start += len;
                length -= len;
            }

            return builder;
        }

        public static StringBuilder AppendNoLoh(this StringBuilder builder, char[] text)
        {
            return AppendNoLoh(builder, text, 0, text.Length);
        }

        public static StringBuilder AppendNoLoh(this StringBuilder builder, char[] text, int start, int length)
        {
            while (length > 0)
            {
                int len = Math.Min(StringBuilderExtensions.MaximumCharCountNoLoh, length);

                builder.Append(text, start, len);
                start += len;
                length -= len;
            }

            return builder;
        }

        public static StringBuilder AppendJoin(this StringBuilder builder, IReadOnlyList<string> values, string separator = ", ")
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (i != 0)
                {
                    builder.Append(separator);
                }

                builder.Append(values[i]);
            }

            return builder;
        }

        public static StringBuilder AppendHex(this StringBuilder builder, byte[] data)
        {
            builder.Append("0x");

            foreach (byte b in data)
            {
                builder.Append(HexDigits[b >> 4]);
                builder.Append(HexDigits[b & 15]);
            }

            return builder;
        }

        public static StringBuilder FrugalAppend(this StringBuilder builder, long value)
        {
            if (value < 0)
            {
                builder.Append('-');

                return builder.FrugalAppend((ulong)(-value));
            }

            return builder.FrugalAppend((ulong)(value));
        }

        public unsafe static StringBuilder FrugalAppend(this StringBuilder builder, ulong value)
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
                builder.Append(buffer[i]);
            }

            return builder;
        }

        public static void WriteToNoLoh(this StringBuilder builder, TextWriter writer)
        {
            char[] buffer = AcquireCharBuffer(4096);

            int length = builder.Length;

            int start = 0;

            while (length > 0)
            {
                int len = Math.Min(buffer.Length, length);

                builder.CopyTo(start, buffer, 0, len);

                writer.Write(buffer, 0, len);
                start += len;
                length -= len;
            }

            buffer.ReleaseCharBuffer();
        }

        public static string FrugalReadToEnd(this TextReader reader)
        {
            char[] buffer = AcquireCharBuffer();

            StringBuilder builder = AcquireBuilder();

            int charCount;
            while ((charCount = reader.Read(buffer, 0, buffer.Length)) != 0)
            {
                builder.Append(buffer, 0, charCount);
            }

            buffer.ReleaseCharBuffer();

            return builder.ToStringAndRelease();
        }

        public static char[] AcquireCharBuffer(int capacity = 4096)
        {
            char[] buffer = Interlocked.Exchange(ref StringBuilderExtensions.reusedBuffer, null);

            if ((buffer == null) || (buffer.Length < capacity))
            {
                buffer = new char[capacity];
            }

            return buffer;
        }

        public static void ReleaseCharBuffer(this char[] buffer)
        {
            if (buffer != null)
            {
                StringBuilderExtensions.reusedBuffer = buffer;
            }
        }
    }
}
