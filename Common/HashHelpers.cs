using System;
using System.Collections.Generic;

namespace FrugalCafe
{
    public static class HashHelpers
    {
        public static readonly int[] Primes24;
        public static readonly int[] Primes16;

        public static readonly int[] OldPrimes = new int[]
        {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71,
            89, 107, 131, 163, 197, 239, 293, 353, 431, 521,
            631, 761, 919, 1103, 1327, 1597, 1931, 2333, 2801, 3371,
            4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591, 17519, 21023,
            25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363,
            156437, 187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403,
            968897, 1162687, 1395263, 1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559,
            5999471, 7199369
        };

        static HashHelpers()
        {
            Primes24 = AddNoLohPrimes(24);
            Primes16 = AddNoLohPrimes(16);
        }

        public static int[] AddNoLohPrimes(int elementSize)
        {
            HashSet<int> combined = new HashSet<int>(HashHelpers.OldPrimes);

            foreach (var p in HashHelpers.NoLohSequence(elementSize))
            {
                combined.Add(p);
            }

            var results = new int[combined.Count];

            combined.CopyTo(results, 0);

            Array.Sort(results);

            return results;
        }

        public static IEnumerable<int> NoLohSequence(int elementSize)
        {
            int candidate = (85000 - 24) / elementSize;

            while (candidate >= 2)
            {
                if (candidate != 2)
                {
                    if ((candidate & 1) == 0)
                    {
                        candidate--;
                    }

                    while (!HashHelpers.IsPrime(candidate))
                    {
                        candidate -= 2;
                    }
                }

                yield return candidate;

                candidate /= 2;
            }
        }

        public static int Combine(this int h1, int h2)
        {
            uint num = (uint)(h1 << 5) | ((uint)h1 >> 27);
            return ((int)num + h1) ^ h2;
        }

        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                for (int divisor = 3; (divisor * divisor) <= candidate; divisor += 2)
                {
                    if ((candidate % divisor) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return (candidate == 2);
        }
    }
}
