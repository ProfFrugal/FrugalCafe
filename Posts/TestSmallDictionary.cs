using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe.Posts
{
    internal class TestSmallDictionary
    {
        public static void Test()
        {
            SmallDictionary<short, int> square = new SmallDictionary<short, int>();

            for (short i = 0; i < 1000; i++)
            {
                square.Add(i, i * i);
            }

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
