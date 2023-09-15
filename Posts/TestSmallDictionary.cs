using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe.Posts
{
    internal class TestSmallDictionary
    {
        private static void GenerateNextPrime()
        {
            int c = 0;

            for (int i = 3; i < 6000; i += 2)
            {
                int p = i;

                while (!HashHelpers.IsPrime(p))
                {
                    p += 2;
                }

                Console.Write("{0,4},", p);

                c++;

                if (c == 20)
                {
                    Console.WriteLine();
                    c = 0;
                }
            }
        }

        public static void Test()
        {
            Dictionary<short, int> t0 = new Dictionary<short, int>();

            Console.WriteLine(HashHelpers.NextPrimes.Length);

            {
                PerfTest.Start();

                Dictionary<short, int> square0 = new Dictionary<short, int>();

                for (short i = 0; i < 1000; i++)
                {
                    square0.Add(i, i * i);
                }

                var copy0 = new Dictionary<short, int>(square0);
                PerfTest.Stop("Dictionary<short, int>", 1);
            }

            PerfTest.Start(null, gc: false);

            SmallDictionary<short, int> square = new SmallDictionary<short, int>();

            for (short i = 0; i < 1000; i++)
            {
                square.Add(i, i * i);
            }

            var copy = new SmallDictionary<short, int>(square);

            PerfTest.Stop("SmallDictionary<short, int>", 1);

            Console.WriteLine(square.Count);
            Console.WriteLine(square[100]);

            for (short i = 0; i < 1000; i++)
            {
                var v = square[i];

                if (v != i * i)
                {
                    Console.WriteLine("{0} {1}", i, v);
                }
            }
        }
    }
}
