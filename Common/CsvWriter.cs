// Copyright (c) 2023 Feng Yuan for https://frugalcafe.beehiiv.com/
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
