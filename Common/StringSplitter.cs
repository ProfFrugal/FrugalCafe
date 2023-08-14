using System;
using System.Threading;

namespace FrugalCafe
{
    public class StringSplitter : OpenList<ValueTuple<int, int>>
    {
        private string _text;

        public void Split(string text, char separator)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.Split(text, separator, 0, text.Length);
        }

        public void Split(string text, char separator, int start, int length)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            _text = text;

            base.Clear(false);

            int limit = start + length;

            for (int i = start; i < limit; i++)
            {
                if (text[i] == separator)
                {
                    base.Add((start, i - start));

                    start = i + 1;
                }
            }

            base.Add((start, limit - start));
        }

        public new Substring this[int index]
        {
            get
            {
                var pos = base[index];

                return new Substring(_text, pos.Item1, pos.Item2);
            }
        }

        public static void Test()
        {
            string phrase = "A quick brown fox jumps over the lazy dog.";

            int count = 1000 * 1000;

            StringSplitter reusedSplitter = new StringSplitter();

            int length1 = 0;
            int length2 = 0;

            PerfTest.MeasurePerf(
                () =>
                {
                    string[] parts = phrase.Split(' ');

                    length1 += parts.Length;
                },
                "string.Split",
                count
                );

            PerfTest.MeasurePerf(
                () =>
                {
                    var splitter = Interlocked.Exchange(ref reusedSplitter, null);

                    if (splitter == null)
                    {
                        splitter = new StringSplitter();
                    }

                    splitter.Split(phrase, ' ');

                    length2 += splitter.Count;

                    reusedSplitter = splitter;
                },
                "StringSplitter",
                count
                );

            Console.WriteLine("{0} {1}", length1 / count, length2 / count);
        }
    }
}
