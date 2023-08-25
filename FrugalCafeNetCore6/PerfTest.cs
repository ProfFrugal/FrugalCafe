using System;
using System.Threading;

namespace FrugalCafe
{
    public static class PerfTest
    {
        static DateTime now;
        static long alloc;

        public static void MeasurePerf(Action action, string name, int repeat)
        {
            PerfTest.Start(action);

            for (int i = 0; i < repeat; i++)
            {
                action();
            }

            PerfTest.Stop(name, repeat, 1);
        }

        public static void Start(Action? warmup = null)
        {
            Console.WriteLine();

            if (warmup != null)
            {
                warmup();
                warmup();
            }

            Thread.Sleep(500);
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
            GC.Collect(2);
            Thread.Sleep(500);

            PerfTest.now = DateTime.UtcNow;

            PerfTest.alloc = GC.GetAllocatedBytesForCurrentThread();
        }

        public static void Stop(string scenario, int count, int repeat = 1)
        {
            double totalTime = (DateTime.UtcNow - PerfTest.now).TotalMilliseconds;
            PerfTest.alloc = GC.GetAllocatedBytesForCurrentThread() - PerfTest.alloc;

            long totalItem = count * repeat;

            if (repeat != 1)
            {
                Console.WriteLine("{0} {1:N2} ms, {2:N0} x {3}", scenario, totalTime, count, repeat);
            }
            else
            {
                Console.WriteLine("{0} {1:N2} ms, {2:N0}", scenario, totalTime, count);
            }

            Console.WriteLine("{0:N3} μs per item", totalTime * 1000 / totalItem);
            Console.WriteLine("Allocation: {0:N0} bytes, {1:N2} per item", alloc, alloc / totalItem);
            Console.WriteLine("Memory usage: {0:N2} mb", GC.GetTotalMemory(false) / 1024 / 1024.0);
        }
    }
}
