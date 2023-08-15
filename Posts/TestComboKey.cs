using System;
using System.Collections.Generic;

namespace FrugalCafe.Posts
{
    internal static class TestComboKey
    {
        public static void Test()
        {
            int count = 5000 * 1000;

            HashSet<Tuple<string, int, char>> set1 = new HashSet<Tuple<string, int, char>>();

            PerfTest.MeasurePerf(
                () => { set1.Add(new Tuple<string, int, char>("Hello", 1, 'a')); },
                "Tuple", count);

            HashSet<ValueTuple<string, int, char>> set2 = new HashSet<ValueTuple<string, int, char>>();

            PerfTest.MeasurePerf(
                () => { set2.Add(new ValueTuple<string, int, char>("Hello", 1, 'a')); },
                "ValueTuple", count);

            HashSet<ComboKey<string, int, char>> set3 = new HashSet<ComboKey<string, int, char>>();

            PerfTest.MeasurePerf(
                () => { set3.Add(new ComboKey<string, int, char>("Hello", 1, 'a')); },
                "ComboKey", count);
        }
    }
}
