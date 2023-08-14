using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace FrugalCafe.Posts
{
    static internal class TestSubstring
    {
        public static void Test()
        {
            string phrase = "A quick brown fox jumps over the lazy dog.";

            phrase = phrase + " " + phrase;

            int count = 1000 * 1000;

            {
                char[] singleSpace = new char[] { ' ' };

                HashSet<StringSegment> unique1 = new HashSet<StringSegment>();

                int uniqueCount1 = 0;

                PerfTest.MeasurePerf(
                    () =>
                    {
                        var sentence = new StringSegment(phrase);

                        foreach (StringSegment word in sentence.Split(singleSpace))
                        {
                            unique1.Add(word);
                        }

                        uniqueCount1 = unique1.Count;
                    },
                    "StringSegment.Split",
                    count
                    );

                Console.WriteLine(uniqueCount1);
            }

            {
                int uniqueCount2 = 0;

                HashSet<Substring> unique2 = new HashSet<Substring>();

                StringSplitter splitter = new StringSplitter();

                PerfTest.MeasurePerf(
                    (Action)(() =>
                    {
                        splitter.Split(phrase, ' ');

                        int c = splitter.Count;

                        for (int i = 0; i < c; i++)
                        {
                            unique2.Add(splitter[i]);
                        }

                        uniqueCount2 = unique2.Count;
                    }),
                    "FrugalCafe.Substring",
                    count
                    );

                Console.WriteLine(uniqueCount2);
            }
        }
    }
}
