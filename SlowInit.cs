using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrugalCafe
{
    internal class SlowInit
    {
        static void ParallelArray()
        {
            int collectionSize = 1000 * 1000;

            int[] myCollection = new int[collectionSize];

            Parallel.For(0, collectionSize, i =>
            {
                int element = InitializeElement(i);
                myCollection[i] = element;
            });
        }

        static void SingleThreadArray()
        {
            int collectionSize = 1000 * 1000;

            int[] myCollection = new int[collectionSize];

            for (int i = 0; i < collectionSize; i++)
            {
                int element = InitializeElement(i);
                myCollection[i] = element;
            }
        }

        static void SingleThreadList()
        {
            int collectionSize = 1000 * 1000;

            List<int> myCollection = new List<int>();

            for (int i = 0; i < collectionSize; i++)
            {
                int element = InitializeElement(i);

                myCollection.Add(element);
            }
        }

        static void ParallelList()
        {
            int collectionSize = 1000 * 1000;

            List<int> myCollection = new List<int>();

            Parallel.For(0, collectionSize, i =>
            {
                int element = InitializeElement(i);

                lock (myCollection)
                {
                    myCollection.Add(element);
                }
            });
        }

        public static void MeasurePerf(Action action, string name, int repeat)
        {
            PerfTest.Start(action);

            for (int i = 0; i < repeat; i++)
            {
                action();
            }

            PerfTest.Stop(name, repeat, 1);
        }

        public static void Main1()
        {
            int count = 32;

            MeasurePerf(SingleThreadList, "SingleThreadList", count);
            MeasurePerf(ParallelList, "ParallelList", count);

            MeasurePerf(SingleThreadArray, "SingleThreadArray", count);
            MeasurePerf(ParallelArray, "ParallelArray", count);
        }

        static int InitializeElement(int i)
        {
            return i * 2;
        }
    }
}
