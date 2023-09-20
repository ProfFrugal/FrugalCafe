using System;
using System.Collections.Generic;
using System.Linq;
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
                    Console.Write((int)ch);
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

        public static void Write(TextWriter writer, IEnumerable<IDictionary<string, string>> records)
        {
            HashSet<string> allKeys = new HashSet<string>();
            SegmentedList<IDictionary<string, string>> copy = null;

            if (!(records is ICollection<IDictionary<string, string>>))
                copy = new SegmentedList<IDictionary<string, string>>(256);

            foreach (IDictionary<string, string> record in records)
            {
                foreach (var kv in record)
                    allKeys.Add(kv.Key);

                if (copy != null)
                    copy.Add(record);
            }
            
            string[] headers = new string[allKeys.Count];
            allKeys.CopyTo(headers, 0);
            Array.Sort<string>(headers, StringComparer.Ordinal);

            if (!CsvConfig<Dictionary<string, string>>.OmitHeaders)
                WriteRow(writer, headers);

            if (copy != null)
                records = copy;

            foreach (IDictionary<string, string> record in records)
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    if (i != 0)
                        writer.Write(',');

                    record.TryGetValue(headers[i], out string value);
                    WriteCsvField(writer, value);
                }

                writer.WriteLine();
            }
        }

        public static void WriteRow(TextWriter writer, IEnumerable<string> row)
        {
            if (writer == null)
            {
                return;
            }
            bool ranOnce = false;
            
            foreach (string field in row)
            {
                WriteItemSeperatorIfRanOnce(writer, ref ranOnce);
                writer.Write(field.ToCsvField());
            }
            
            writer.Write(CsvConfig.RowSeparatorString);
        }

        internal static void WriteItemSeperatorIfRanOnce(TextWriter writer, ref bool ranOnce)
        {
            if (ranOnce)
            {
                writer.Write(CsvConfig.ItemSeperatorString);
            }
            else
            {
                ranOnce = true;
            }
        }
    }
}
