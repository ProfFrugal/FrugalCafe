using System;
using System.Collections.Concurrent;
using System.Text;

namespace FrugalCafeNetCore6
{
    internal class TestValueStringBuilder
    {
        public static void Test()
        {
            var map = new Dictionary<string, string>();

            int count = 60000;

            for (int i = 0; i < count; i++)
            {
                map.Add("key_" + i, (i * i).ToString());
            }

            string result = string.Join(",", map.Select(kv => kv.Key + "=" + kv.Value));

            count = 100;

            for (int test = 0; test < 2; test++)
            {
                List<Task> tasks = new List<Task>();

                ConcurrentDictionary<int, bool> set = new ConcurrentDictionary<int, bool>();

                for (int i = 0; i < count; i++)
                {
                    tasks.Add(Task.Run(() =>
                    {
                        set[Thread.CurrentThread.ManagedThreadId] = true;

                        if (test == 0)
                        {
                            result = OldJoin(",", map.Select(kv => kv.Key + "=" + kv.Value));
                        }
                        else
                        {
                            result = string.Join(",", map.Select(kv => kv.Key + "=" + kv.Value));
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("{0:N0}", result.Length);
                Console.WriteLine("{0} threads, {1:N3} mb", set.Count, GC.GetTotalMemory(true) / 1024 / 1024.0);
            }
        }

        static string OldJoin(string separator, IEnumerable<string> list)
        {
            StringBuilder builder = new StringBuilder();

            bool first = true;

            foreach (var v in list)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    builder.Append(separator);
                }

                builder.Append(v);
            }

            return builder.ToString();
        }
    }
}
