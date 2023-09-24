using System;
using System.IO;
using System.Text;

namespace FrugalCafe
{
    public class CsvWriter : SimpleStreamWriter
    {
        const char ColumnSeparator = ',';
        const char Quote = '"';

        public CsvWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
        }

        public void WriteQuote(string value)
        {
            if (value != null)
            {
                if (CsvWriter.NeedQuote(value))
                {
                    WriteChar(CsvWriter.Quote);

                    foreach (char ch in value)
                    {
                        if (ch == '"')
                        {
                            WriteChar(ch);
                        }

                        WriteChar(ch);
                    }

                    WriteChar(CsvWriter.Quote);
                }
                else
                {
                    WriteString(value);
                }
            }
        }

        public static bool NeedQuote(string value)
        {
            foreach (char ch in value)
            {
                switch (ch)
                {
                    case '\r':
                    case '\n':
                    case ',':
                    case '"':
                        return true;
                }
            }

            return false;
        }

        public void WriteData(string value)
        {
            WriteChar(CsvWriter.ColumnSeparator);
            WriteQuote(value);
        }

        public void WriteData(long value)
        {
            WriteChar(CsvWriter.ColumnSeparator);
            Write(value);
        }

        public void WriteData(double value)
        {
            WriteChar(CsvWriter.ColumnSeparator);
            WriteString(value.ToString());
        }

        public void WriteData(double value, string format, bool first = false)
        {
            if (!first)
            {
                WriteChar(CsvWriter.ColumnSeparator); 
            }

            WriteString(value.ToString(format));
        }
    }
}
