using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace FrugalCafe
{
    public static class Json
    {
        private static readonly Dictionary<string, string> data;
        static Json()
        {
            data = new Dictionary<string, string>();

            for (int i = 0; i < 2000; i++)
            {
                data[i.ToString()] = "A quick brown fox jumps over the lazy dog.";
            }
        }

        public static string ToJson<T>(this T data)
        {
            StringWriter stringWriter = new StringWriter();
            new JsonSerializer().Serialize(new JsonTextWriter(stringWriter), data);

            return stringWriter.ToString();
        }

        private static readonly Encoding UTF8NoBOM = new UTF8Encoding(false, true);

        private static MemoryStream reusedStream;

        public static void PostJson<T>(this T data, Action<byte[], int> post)
        {
            MemoryStream stream = Interlocked.Exchange(ref Json.reusedStream, null);

            if (stream != null)
            {
                stream.SetLength(0);
            }
            else
            {
                stream = new MemoryStream(1024);
            }

            using (StreamWriter writer = new StreamWriter(stream, Json.UTF8NoBOM, 256, leaveOpen: true))
            {
                new JsonSerializer().Serialize(new JsonTextWriter(writer), data);
            }

            post(stream.GetBuffer(), (int)stream.Length);

            Json.reusedStream = stream;
        }

        public static void Test()
        {
            byte[] output1 = Encoding.UTF8.GetBytes(data.ToJson());

            data.PostJson((arry, length) => 
            {
                Debug.Assert(length == output1.Length);

                for (int i = 0; i < length; i++)
                {
                    if (output1[i] != arry[i])
                    {
                        Debug.Assert(false);
                    }
                }

                Console.WriteLine("Output {0:N0} bytes, same", length);
            });
        }

        public static void PerfTest()
        {
            int count = 1000;

            byte[] output1 = Encoding.UTF8.GetBytes(data.ToJson());
            output1 = Encoding.UTF8.GetBytes(data.ToJson());

            FrugalCafe.PerfTest.Start();

            for (int i = 0; i < count; i++)
            {
                output1 = Encoding.UTF8.GetBytes(data.ToJson());
            }

            FrugalCafe.PerfTest.Stop("Before", count, 1);

            data.PostJson((arry, length) => { output1 = arry; });
            data.PostJson((arry, length) => { output1 = arry; });

            FrugalCafe.PerfTest.Start();

            for (int i = 0; i < count; i++)
            {
                data.PostJson((arry, length) => { output1 = arry; });
            }

            FrugalCafe.PerfTest.Stop("After", count, 1);
        }

        public static string Stringify(string headline, string body = "", Dictionary<string, string> headers = null)
        {
            if ((headers == null) || (headers.Count == 0)) 
            { 
                if (string.IsNullOrEmpty(body))
                {
                    return headline + "\r\n\r\n";
                }

                return headline + "\r\n\r\n" + body + "\r\n";
            }

            string[] list = new string[5 + headers.Count * 4];

            int count = 0;

            list[count++] = headline;
            list[count++] = "\r\n";

            foreach (KeyValuePair<string, string> header in headers)
            {
                list[count++] = header.Key;
                list[count++] = ": ";
                list[count++] = header.Value;
                list[count++] = "\r\n";
            }

            list[count++] = "\r\n";

            if (!string.IsNullOrEmpty(body))
            {
                list[count++] = headline;
                list[count++] = "\r\n";
            }

            return string.Join(string.Empty, list, 0, count);
        }

        public static StringBuilder SplitAppend(this StringBuilder builder, string text)
        {
            if (text != null)
            {
                int length = text.Length;
                int pos = 0;

                while (length > 0)
                {
                    int len = Math.Min(42000, length);

                    builder.Append(text, pos, len);
                    pos += len;
                    length -= len;
                }
            }

            return builder;
        }
    }
}
