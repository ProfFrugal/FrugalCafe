using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FrugalCafe
{
    internal class TestSegmentedList
    {
        public static void Test()
        {
            var seglist = new SegmentedList<int>(segmentSize: 127);

            int count = 200;

            for (int i = 0; i < count; i++)
            {
                seglist.Add(i);
            }

            var list = new List<int>();

            for (int i = 0; i < count; i++)
            {
                list.Add(i);
            }

            int sum1 = 0;

            for (int i = 0; i < count; i++)
            {
                sum1 += list[i];

                Debug.Assert(list[i] == seglist[i]);
            }

            Debug.Assert(list.Count == seglist.Count);

            foreach (var i in seglist)
            {
                sum1 -= seglist[i];
            }

            Debug.Assert(sum1 == 0);
        }

        public static void Perf()
        {
            int size = 85000 / 4;

            int count = 1000 * 100;

            PerfTest.MeasurePerf(
                () => 
                { 
                    List<int> list = new List<int>();

                    for (int i = 0; i < size; i++)
                    {
                        list.Add(i);
                    }
                }, 
                "List<int>", count);

            PerfTest.MeasurePerf(
                () =>
                {
                    SegmentedList<int> list = new SegmentedList<int>(segmentSize: 1024);

                    for (int i = 0; i < size; i++)
                    {
                        list.Add(i);
                    }
                },
                "SegmentedList<int>", count);

            PerfTest.MeasurePerf(
                () =>
                {
                    List<int> list = new List<int>(size);

                    for (int i = 0; i < size; i++)
                    {
                        list.Add(i);
                    }
                },
                "List<int>(size)", count);
        }
    }
}
