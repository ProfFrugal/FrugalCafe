using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    internal class TestStringBuilderCache
    {
        public static void Test()
        {
            string pangram = "The quick brown fox jumps over the lazy dog";

            string paragraph = string.Format("{0} {0} {0} {0} {0} {0} {0} {0} {0}", pangram);

            Console.WriteLine(paragraph);

            int count = 1000 * 1000;

            PerfTest.MeasurePerf(
                () => { paragraph = string.Format("{0} {0} {0} {0} {0} {0} {0} {0} {0}", pangram); },
                "string.Format", count);

            PerfTest.MeasurePerf(
                () => { paragraph = "{0} {0} {0} {0} {0} {0} {0} {0} {0}".FrugalFormat(pangram); },
                "FrugalFormat", count);
        }
    }
}
