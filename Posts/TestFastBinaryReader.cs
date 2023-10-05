using System;
using System.Diagnostics;
using System.IO;

namespace FrugalCafe.Posts
{
    internal class TestFastBinaryReader
    {
        public static void Test()
        {
            string pangram = "A quick brown fox jumps over the lazy dog.";

            byte[] data = null;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(stream);

                writer.Write(123);
                writer.Write(pangram);
                writer.Write(456);
                writer.Flush();

                data = stream.ToArray();
            }

            string result = null;

            for (int i = 0; i < 3; i++)
            {
                using (MemoryStream stream = new MemoryStream(data))
                {
                    FastBinaryReader reader = new FastBinaryReader(stream);

                    int first = reader.ReadInt32();
                    string s = reader.ReadString();
                    int last = reader.ReadInt32();

                    if (result != null)
                    {
                        Debug.Assert(object.ReferenceEquals(result, s));
                    }

                    Debug.Assert(first == 123);
                    Debug.Assert(last == 456);

                    result = s;
                }
            }
        }
    }
}
