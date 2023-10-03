using System;
using System.Diagnostics;

namespace FrugalCafe.Posts
{
    internal class TestStringTable
    {
        public static void Test()
        {
            StringTable table = StringTable.Shared;

            for (int i = 0; i < 10; i++)
            {
                string cafe1 = table.GetOrAdd("FrugalCafe" + i);
                string cafe2 = table.GetOrAdd("FrugalCafe" + i);

                Debug.Assert(object.ReferenceEquals(cafe1, cafe2));

                Console.WriteLine(cafe1);
                Console.WriteLine(cafe2);
            }
        }
    }
}
