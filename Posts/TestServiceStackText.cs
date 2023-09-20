using System;
using System.IO;
using ServiceStack;
using ServiceStack.Text;
using ServiceStack.Text.Common;

namespace FrugalCafe.Posts
{
    internal static class TestServiceStackText
    {
        public static void Test()
        {
            Console.WriteLine(CsvConfig.ItemDelimiterString);

            foreach (string s in CsvConfig.EscapeStrings)
            {
                foreach (char ch in s)
                {
                    Console.Write((int) ch);
                    Console.Write(' ');
                }

                Console.WriteLine();
            }

            string text = "Dictionary<string, int>";

            string output = text.ToCsvField();

            int count = 5000 * 1000;

            StringWriter writer = new StringWriter();

            PerfTest.MeasurePerf(
                () => 
                {
                    writer.GetStringBuilder().Clear();
                    writer.Write(text.ToCsvField());
                },
                "ToCsvField",
                count);

            PerfTest.MeasurePerf(
                () =>
                {
                    writer.GetStringBuilder().Clear();
                    WriteCsvField(writer, text);
                },
                "WriteCsvField",
                count);

            Console.WriteLine(text);
            Console.WriteLine(output);
        }

        public static void WriteCsvField(TextWriter writer, string text)
        {
            char escape = FindEscapeChar(text);

            if (escape != char.MinValue)
            {
                writer.Write('"');

                if (escape == '"')
                {
                    foreach (char ch in text)
                    {
                        if (ch == '"')
                        {
                            writer.Write(ch);
                        }

                        writer.Write(ch);
                    }
                }
                else
                {
                    writer.Write(text);
                }

                writer.Write('"');
            }
            else
            {
                writer.Write(text);
            }
        }

        public static char FindEscapeChar(string value)
        {
            char result = char.MinValue;

            if (value != null)
            {
                int length = value.Length;

                if (length != 0)
                {
                    char c0 = value[0];

                    if (c0 == JsWriter.ListStartChar || c0 == JsWriter.MapStartChar)
                    {
                        result = c0;
                    }

                    foreach (char ch in value)
                    {
                        switch (ch)
                        {
                            case '"':
                                return ch;

                            case ',':
                            case '\r':
                            case '\n':
                                result = ch;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
