using FrugalCafe.Posts;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace FrugalCafe
{
    internal class Program
    {
        /// <summary>
        /// Put double quotes around 'str' if necessary (handles quotes quotes.  
        /// </summary>
        public static string Quote(string str)
        {
            if (str.IndexOf('"') >= 0)
            {
                // Replace any " with \"  (and any \" with \\" and and \\" with \\\"  ...)
                str = Regex.Replace(str, "\\*\"", @"\$1");
            }
            return "\"" + str + "\"";
        }

        public static string Quote1(string str)
        {
            if (str.IndexOf('"') >= 0)
            {
                // Replace any " with \"  (and any \" with \\" and and \\" with \\\"  ...)
                str = str.Replace("\"", "\\\"");
            }
            return "\"" + str + "\"";
        }

        public List<Product> GetProducts()
        {
            return new List<Product>();
        }

        public List<Product> OrderByWithThenByMethod()
        {
            var productList = GetProducts();

            productList = productList.OrderBy(x => x.Name)
                .ThenBy(x => x.ListPrice)
                .ThenBy(x => x.Size)
                .ToList();

            return productList;
        }

        public static KeyValuePair<char, int> GetMostRepeatedCharacterWithCount(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text cannot be null or empty");
            }

            var charDictionary = new Dictionary<char, int>();

            foreach (char chr in text)
            {
                if (chr != ' ')
                {
                    charDictionary[chr] = charDictionary.TryGetValue(chr, out int value) ? ++value : 1;
                }
            }

            var max = default(KeyValuePair<char, int>);

            foreach(var kv in charDictionary)
            {
                if (kv.Value > max.Value)
                {
                    max = kv;
                }
            }

            return max;
        }

        static void Main(string[] args)
        {
            NGram.Test();
            return;

            FrugalCafe.YouTube.String1.Test();

            TestSmallArrayPool.Test();

            TestEventSource.Test();
            return;

            TestFastBinaryReader.Test();

            TestStringTable.Test();

            string title = "FrugalCafe";

            for (int i = 0; i < 10; i++)
            {
                title = title + i;

                int hash1 = title.GetHashCode();
                int hash2 = new Substring(title).GetHashCode();

                System.Diagnostics.Debug.Assert(hash1 == hash2);

                Console.WriteLine("{0:N0} {1:N1}", hash1, hash2);
            }

            var writer = new StreamWriter(new MemoryStream());

            Console.WriteLine(Quote1("Microsoft"));
            Console.WriteLine(Quote1("Mic\"rosoft"));

            TestServiceStackText.Test();
            return;

            Customer.Test();
            return;

            TestSmallDictionary.Test();
            return;

            BillionObjects.Test();

            return;

            TestSimpleHashSet.Test();
            return;

            TestIsPrime.Test();
            return;

            TestDictionaryArray.Test(args[0]);
            return;

            TestInplaceOrderBy.Test();

            TestOptimalFormat.Test();
            return;

            TestISimpleStringBuilder.Test();

            (new TestICSharpDecompiler()).Test(typeof(string));

            return;

            TestStringBuilderCache.Test();
            return;

            //TestEnumHelper.TestCase();
            //return;

            TestPCCategory.Test();
            return;

            TestSegmentedList.Perf();

            return;

            TestMemoryCache.Test();

            return;

            StringComparerTest.Test();

            return;

            TestComboKey.Test();

            return;

            TestSubstring.Test();

            return;

            StringSplitter.Test();

            return;

            StringCompare.PerfTest();

            return;

            RegularExpression.Test();

            return;

            TestJsonContractResolver.Test();

            return;

            StringHelpers.Test(1000 * 1000);

            return;

            MemoryCacheLeak.Test();

            SlowInit.Main1();

            return;

            DictionaryCorruption.Test();

            return;

            LinqIsNotQuick.TestLinq();

            Json.Test();

            Console.WriteLine();

            Json.PerfTest();
        }
    }
}
