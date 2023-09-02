using System;

namespace FrugalCafe.Posts
{
    internal class TestIsPrime
    {
        public static void NoLohSequence(int elementSize, int[] primes)
        {
            Console.WriteLine();

            foreach (var p in HashHelpers.NoLohSequence(elementSize)) 
            {
                Console.WriteLine("{0,6:N0} {1,7:N0}", p, p * elementSize + 24);
            }

            Console.WriteLine();

            foreach (var p in primes)
            {
                int size = p * elementSize + 24;

                if (size >= 85000)
                {
                    break;
                }

                Console.Write("{0,6:N0} x {2} {1,7:N0}", p, size, elementSize);

                if (Array.IndexOf(HashHelpers.OldPrimes, p) < 0)
                {
                    Console.Write(" *");
                }

                Console.WriteLine();
            }
        }

        public static void Test()
        {
            NoLohSequence(24, HashHelpers.Primes24);
            NoLohSequence(16, HashHelpers.Primes16);
        }
    }
}
