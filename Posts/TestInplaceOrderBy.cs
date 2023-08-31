using FrugalCafe.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FrugalCafe
{
    internal class TestInplaceOrderBy
    {
        public static void Test()
        {
            var data = "A quick brown fox jumps over the lazy dog".Split(' ');

            List<string> pangram = new List<string>();

            for (int i = 0; i < 32; i++)
            {
                pangram.AddRange(data);
            }

            int result = 0;

            int count = 1000 * 100;

            PerfTest.MeasurePerf(
                () =>
                {
                    result = pangram.OrderByDescending((k) => k, StringComparer.Ordinal).Count();
                },
                "Linq.OrderBy", count);

            PerfTest.MeasurePerf(
                () =>
                {
                    List<string> copy = new List<string>(pangram);

                    copy.InplaceOrderByDescending((k) => k, StringComparer.Ordinal);
                    result = copy.Count;
                },
                "InplaceOrderByDescending + selector", count);

            PerfTest.MeasurePerf(
                () =>
                {
                    List<string> copy = new List<string>(pangram);

                    copy.InplaceOrderByDescending(StringComparer.Ordinal);
                    result = copy.Count;
                },
                "InplaceOrderByDescending", count);

        }
    }
}
