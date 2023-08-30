﻿using FrugalCafe.Posts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FrugalCafe
{
    internal class Program
    {
        static void Main(string[] args)
        {
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
