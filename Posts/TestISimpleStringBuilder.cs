using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FrugalCafe
{
    public struct TextWriterWrapper : ISimpleStringBuilder
    {
        private TextWriter _writer;
        public TextWriterWrapper(TextWriter writer) 
        { 
            _writer = writer;
        }

        public void Append(string value)
        {
            _writer.Write(value);
        }

        public void Append(string format, int index)
        {
            _writer.Write(format[index]);
        }

        public void Pad(int repeat)
        {
            for (int i = 0; i < repeat; i++)
            {
                _writer.Write(' ');
            }
        }
    }

    internal class TestISimpleStringBuilder
    {
        public static void Test()
        {
            string format = "A {0} fox jumps over the {1} dog. {2:N2}";

            string out1 = string.Format(format, "quick", "brown", format.Length);

            StringWriter writer = new StringWriter();

            (new TextWriterWrapper(writer)).AppendFormat(
                format, new ParamsArray<object>("quick", "brown", format.Length));

            string out2 = writer.ToString();

            Console.WriteLine(out1);
            Console.WriteLine(out2);
        }
    }
}
