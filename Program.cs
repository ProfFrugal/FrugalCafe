using FrugalCafe.Posts;
using System;

namespace FrugalCafe
{
    internal class Program
    {

        static void Main(string[] args)
        {
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

            //TestEnumHelper.Test();
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
