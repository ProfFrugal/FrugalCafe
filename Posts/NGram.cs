using System;
using System.Collections.Generic;
using System.Linq;

namespace FrugalCafe.Posts
{
    public class NGram
    {
        public static double CosineSimilarity(string text1, string text2, int n)
        {
            if (text1 == null || text1.Length < n)
            {
                return 0;
            }

            if (text2 == null || text2.Length < n)
            {
                return 0;
            }

            if (text1 == text2)
            {
                return 1;
            }

            var ngram1 = new Dictionary<ulong, int>(text1.Length - n + 1);
        
            NGram.GetNgrams(text1, n, ngram1);

            var ngram2 = new Dictionary<ulong, int>(text2.Length - n + 1);

            foreach (var k in ngram1.Keys)
            {
                ngram2[k] = 0;
            }

            NGram.GetNgrams(text2, n, ngram2);

            double dotProduct = 0, magnitude1 = 0, magnitude2 = 0;
            int count1 = ngram1.Count;

            using (var enum1 = ngram1.Values.GetEnumerator())
            {
                foreach (var y in ngram2.Values)
                {
                    if (count1-- > 0)
                    {
                        enum1.MoveNext();
                        int x = enum1.Current;
                        dotProduct += x * y;
                        magnitude1 += x * x;
                    }

                    magnitude2 += y * y;
                }
            }

            return (magnitude1 > 0 && magnitude2 > 0) ?
                dotProduct / Math.Sqrt(magnitude1 * magnitude2) : 0;
        }

        static void GetNgrams(string text, int n, Dictionary<ulong, int> ngrams)
        {
            if (n < 1 || n > 4)
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }

            for (int i = 0; i <= text.Length - n; i++)
            {
                ulong gram = 0;

                for (int p = 0; p < n; p++)
                {
                    gram = (gram << 16) + (ushort)text[i + p];
                }

                ngrams.TryGetValue(gram, out int count);
                ngrams[gram] = count + 1;
            }
        }

        /// Original code
        static List<string> GetNGrams(string text, int n)
        {
            var ngrams = new List<string>();

            for (int i = 0; i <= text.Length - n; i++)
                ngrams.Add(text.Substring(i, n));

            return ngrams;
        }

        /// Original code
        static double CosineSimilarity(List<string> list1, List<string> list2) 
        { 
            var allGrams = list1.Union(list2).ToList();

            var vector1 = allGrams.Select(g => list1.Count(x => x ==g)).ToArray();
            var vector2 = allGrams.Select(g => list2.Count(x => x == g)).ToArray();

            double dotProduct = vector1.Zip(vector2, (a, b) => a * b).Sum();
            double magnitude1 = Math.Sqrt(vector1.Sum(v => v * v));
            double magnitude2 = Math.Sqrt(vector2.Sum(v => v * v));

            return (magnitude1 > 0 && magnitude2 > 0) ?
                dotProduct / (magnitude1 * magnitude2) : 0;
        }

        public static void TestCase(string text1, string text2)
        {
            double cos1 = 0, cos2 = 0;
            int count = 100000;

            PerfTest.MeasurePerf(
                () =>
                {
                    var list1 = GetNGrams(text1, 3);
                    var list2 = GetNGrams(text2, 3);
                    cos1 = CosineSimilarity(list1, list2);
                },
                "Elliot One - CosineSimilarity", count);

            PerfTest.MeasurePerf(
                () =>
                {
                    cos2 = CosineSimilarity(text1, text2, 3);
                },
                "Feng Yuan - CosineSimilarity", count);

            Console.WriteLine();
            Console.WriteLine("{0} {1}", text1.Length, text2.Length);
            Console.WriteLine(cos1);
            Console.WriteLine(cos2);
        }

        public static void Test()
        {
            string text1 = "software engineer";
            string text2 = "senior software engineer";

            TestCase(text1, text2);

            text1 = "Man Fired From Microsoft After 22 Years Becomes Goose Farmer";
            text2 = "Software Performance Architect Fired From Microsoft After 23 Years Becomes Goose Farmer";

            TestCase(text1, text2);
        }
    }
}
