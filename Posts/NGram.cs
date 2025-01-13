using System;
using System.Collections.Generic;
using System.Linq;

namespace FrugalCafe.Posts
{
    public class NGram
    {
        private Dictionary<ulong, int> ngram1 = new Dictionary<ulong, int>();
        private Dictionary<ulong, int> ngram2 = new Dictionary<ulong, int>();

        public double CosineSimilarity(string text1, string text2, int n)
        {
            this.ngram1.Clear();
            NGram.GetNgrams(text1, n, this.ngram1);
            this.ngram2.Clear();

            foreach (var k in this.ngram1.Keys)
            {
                this.ngram2[k] = 0;
            }

            NGram.GetNgrams(text2, n, this.ngram2);

            double dotProduct = 0, magnitude1 = 0, magnitude2 = 0;
            int count1 = this.ngram1.Count;

            using (var enum1 = this.ngram1.Values.GetEnumerator())
            {
                foreach (var y in this.ngram2.Values)
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

        public static void Test()
        {
            string text1 = "software engineer";
            string text2 = "senior software engineer";

            var ngram = new NGram();

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
                    cos2 = ngram.CosineSimilarity(text1, text2, 3);
                },
                "Feng Yuan - CosineSimilarity", count);

            Console.WriteLine();
            Console.WriteLine(cos1);
            Console.WriteLine(cos2);
        }
    }
}
