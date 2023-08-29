using System;
using System.Diagnostics;

namespace FrugalCafe.Posts
{
    internal class TestOptimalFormat
    {
        public static void Test()
        {
            string pangram = "The quick brown fox jumps over the lazy dog";

            string formatString = "{0} {0} {0} {0} {0} {0} {0} {0} {0}";

            string paragraph0 = string.Format(formatString, pangram);

            Console.WriteLine(paragraph0.Length);

            string paragraph1 = formatString.OptimalFormat(pangram);

            Debug.Assert(object.ReferenceEquals(pangram.OptimalFormat("x"), pangram));

            Debug.Assert(object.ReferenceEquals("{0}".OptimalFormat(pangram), pangram));

            Debug.Assert(paragraph0.Equals(paragraph1));

            int count = 2000 * 1000;

            PerfTest.MeasurePerf(
                () => { paragraph0 = string.Format(formatString, pangram); },
                "string.Format", count);

            PerfTest.MeasurePerf(
                () => { paragraph0 = formatString.OptimalFormat(pangram); },
                "OptimalFormat", count);
        }
    }
}
