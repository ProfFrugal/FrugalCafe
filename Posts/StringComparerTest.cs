using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrugalCafe.Posts
{
    internal static class StringComparerTest
    {
        public static void Test()
        {
            string phrase = "A quick brown fox jumps over the lazy dog.";

            int count = 100 * 1000 * 1000;

            PerfTest.MeasurePerf(() => phrase.GetHashCode(), "string.GetHashCode", count);

            PerfTest.MeasurePerf(() => StringComparer32.Instance.GetHashCode(phrase), "StringCompare32.GetHashCode", count);
        }
    }
}
