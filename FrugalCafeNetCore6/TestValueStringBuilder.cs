using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

namespace FrugalCafeNetCore6
{
    internal class TestValueStringBuilder
    {
        public static List<object> GenerateData(int count)
        {
            var map = new Dictionary<string, string>();

            for (int i = 0; i < count; i++)
            {
                map.Add("key_" + i, (i * i).ToString());
            }

            List<object> data = new List<object>(count);

            foreach (var kv in map)
            {
                data.Add(kv.Key + "=" + kv.Value);
            }

            return data;
        }

        public static void Test()
        {
            List<object> data = GenerateData(60000);

            string result = string.Join(",", data);

            int count = 100;

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
                            result = DotNetFrameworkJoin(",", data);
                        }
                        else
                        {
                            result = string.Join(",", data);
                        }
                    }));
                }

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("{0:N0}", result.Length);
                Console.WriteLine("{0} threads, {1:N3} mb", set.Count, GC.GetTotalMemory(true) / 1024 / 1024.0);
            }
        }

        public static void PerfTest()
        {
            List<object> data = GenerateData(60000);

            int repeat = 1000;

            string result = string.Empty;

            FrugalCafe.PerfTest.MeasurePerf(
                () => 
                {
                    result = string.Join(",", data);
                },
                "string.Join using ValueStringBuilder", repeat);

            Console.WriteLine("{0:N0} chars", result.Length);

            FrugalCafe.PerfTest.MeasurePerf(
                () =>
                {
                    result = LargeBuilderJoin(",", data);
                },
                "LargeBuilderJoin", repeat);

            Console.WriteLine("{0:N0} chars", result.Length);

            if (largeStringPool.TryPeek(out StringBuilder? builder))
            {
                Console.WriteLine("Builder capacity: {0:N0} chars", builder.Capacity);
            }
        }

        static ConcurrentQueue<StringBuilder> largeStringPool = new ConcurrentQueue<StringBuilder>();

        public static StringBuilder AcquireLargeBuilder()
        {
            if (largeStringPool.TryDequeue(out StringBuilder? builder))
            {
                builder.Clear();
            }
            else
            {
                builder = new StringBuilder();
            }

            return builder;
        }

        public static void ReleaseLargeBuilder(StringBuilder builder) 
        {
            if (builder != null)
            {
                largeStringPool.Enqueue(builder);
            }
        }

        static string LargeBuilderJoin(string separator, IEnumerable<object> list)
        {
            StringBuilder? builder = AcquireLargeBuilder();

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

            string result = builder.ToString();

            ReleaseLargeBuilder(builder);

            return result;
        }

        [ThreadStatic]
        static StringBuilder? reusedBuilder;

        static string DotNetFrameworkJoin(string separator, IEnumerable<object> list)
        {
            StringBuilder? builder = reusedBuilder;

            reusedBuilder = null;

            if (builder == null)
            {
                builder = new StringBuilder();
            }
            else
            {
                builder.Clear();
            }

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

            string result = builder.ToString();

            if (builder.Capacity < 360)
            {
                reusedBuilder = builder;
            }

            return result;
        }
    }
}
