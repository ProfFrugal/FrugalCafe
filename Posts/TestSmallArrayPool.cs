using System;
using System.Collections.Generic;

namespace FrugalCafe.Posts
{
    public class TestSmallArrayPool
    {
        public static void Test()
        {
            var pool = SmallArrayPool<byte>.Shared;

            int length = pool.GetTotalLength(out int max);

            Console.WriteLine("{0} {1}", length, max);

            var buffer = pool.Rent(1024);

            pool.Return(buffer);

            length = pool.GetTotalLength(out max);

            Console.WriteLine("{0} {1}", length, max);

            for (int i = 0; i < 2; i++)
            {
                List<byte[]> list = new List<byte[]>();

                for (int j = 0; j < 32; j++)
                {
                    list.Add(pool.Rent(2048 * (i + 1)));
                }

                foreach (var a in list)
                {
                    pool.Return(a);
                }

                length = pool.GetTotalLength(out max);

                Console.WriteLine("{0} {1}", length, max);
            }
        }
    }
}
