using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace FrugalCafe
{
    internal static class DictionaryCorruption
    {
        public static void Test()
        {
            for (int repeat = 0; repeat < 1024 * 256; repeat++)
            {
                int count = 128;

                Stack<int> stack = new Stack<int>();

                for (int i = 0; i < count; i++)
                {
                    stack.Push(i);
                }

                Dictionary<int, int> cache = new Dictionary<int, int>();

                Exception exception = null;

                Parallel.ForEach(stack, (i) =>
                {
                    try
                    {
                        cache.Add(i, Thread.CurrentThread.ManagedThreadId);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                });

                HashSet<int> unique = new HashSet<int>();

                try
                {
                    foreach (var t in cache.Values)
                    {
                        unique.Add(t);
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                }

                Console.WriteLine("{0}, {1} '{2}'", cache.Count, unique.Count, exception);
            }
        }
    }

    internal static class DictionaryCorruption1
    {
        public static void Test()
        {
            for (int repeat = 0; repeat < 128; repeat++)
            {
                int count = 1024;

                Exception exception = null;

                Dictionary<int, int> cache = new Dictionary<int, int>();

                List<Thread> threads = new List<Thread>(); 
                
                for (int i = 0; i < count; i++)
                {
                    threads.Add(new Thread(() =>
                    {
                        try
                        {
                            cache[cache.Count] = Thread.CurrentThread.ManagedThreadId;
                        }
                        catch (Exception e)
                        {
                            exception = e;
                        }
                    }));
                }

                foreach (var t in threads)
                {
                    t.Start();
                }

                Thread.Sleep(100);

                Console.WriteLine("{0} {1}, '{2}'", repeat, cache.Count, exception);
            }

            Console.WriteLine("Press enter to finish");

            Console.ReadLine();
        }
    }
}
