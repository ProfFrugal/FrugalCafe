using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    internal class TestDictionaryArray
    {
        public static void Test(string arg)
        {
            int count = 1000 * 1000;
            int repeat = 10;

            if (arg == "0")
            {
                PerfTest.Start();

                for (int r = 0; r < repeat; r++)
                {
                    DictionaryArray<string, Guid> bigDict = new DictionaryArray<string, Guid>(1103);

                    for (int i = 0; i < count; i++)
                    {
                        Guid guid = Guid.NewGuid();

                        bigDict[guid.ToString()] = guid;
                    }
                }

                PerfTest.Stop("StringDictionaryArray", repeat);
            }
            else
            {
                PerfTest.Start();

                for (int r = 0; r < repeat; r++)
                {
                    Dictionary<string, Guid> bigDict = new Dictionary<string, Guid>();

                    for (int i = 0; i < count; i++)
                    {
                        Guid guid = Guid.NewGuid();

                        bigDict[guid.ToString()] = guid;
                    }
                }

                PerfTest.Stop("Dictionary<string, Guid>", repeat);
            }
        }
    }
}
