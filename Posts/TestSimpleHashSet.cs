using System;
using System.Collections.Generic;

namespace FrugalCafe
{
    internal class TestSimpleHashSet
    {
        public static void Test()
        {
            SimpleHashSet<string> set = new SimpleHashSet<string>();

            int limit = 0;

            foreach(var p in HashHelpers.NoLohSequence(16))
            {
                limit = p;
                break;
            }

            Console.WriteLine("SimpleHashSet<string>: Largest prime number without going into LOH: {0:N0}", limit);

            for (int i = 0; i < limit; i++)
            {
                string guid = Guid.NewGuid().ToString();

                long alloc = GC.GetAllocatedBytesForCurrentThread();

                set.Add(guid);

                alloc = GC.GetAllocatedBytesForCurrentThread() - alloc;

                if (alloc != 0)
                {
                    Console.WriteLine("  {0,4} {1,8:N0} bytes, {2:N0}", i + 1, alloc, set.Capacity);
                }
            }

            Console.WriteLine("Count: {0:N0}", set.Count);

            {
                Console.WriteLine();
                Console.WriteLine("HashSet<string>");

                HashSet<string> set1 = new HashSet<string>();

                for (int i = 0; i < limit; i++)
                {
                    string guid = Guid.NewGuid().ToString();

                    long alloc = GC.GetAllocatedBytesForCurrentThread();

                    set1.Add(guid);

                    alloc = GC.GetAllocatedBytesForCurrentThread() - alloc;

                    if (alloc != 0)
                    {
                        Console.WriteLine("  {0,4} {1,8:N0} bytes", i + 1, alloc);
                    }
                }

                Console.WriteLine("Count: {0:N0}", set1.Count);

            }
        }
    }
}
