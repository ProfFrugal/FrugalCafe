using FrugalCafe;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FrugalCafeNetCore6
{
    internal class TestAny
    {
        public static void Test()
        {
            string pangram = "A quick brown fox jumps over the lazy dog.";

            bool any = pangram.Any();

            for (int i = 0; i < 10; i++)
            {
                any = pangram.Any();

                if (i == 0)
                {
                    Debugger.Break();
                }
            }
        }

        public static void PerfTest()
        {
            string pangram = "A quick brown fox jumps over the lazy dog.";

            bool any = pangram.Any();

            int count = 10 * 1000 * 1000;

            FrugalCafe.PerfTest.MeasurePerf(() => { any = pangram.Any(); }, "String.Any", count);
        }
    }
}
