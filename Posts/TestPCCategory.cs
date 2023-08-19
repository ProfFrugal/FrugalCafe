using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    internal class TestPCCategory
    {
        public static void DumpCategory(string categoryName)
        {
            DateTime now = DateTime.UtcNow;

            var category = new PCCategory(categoryName);

            int count = category.Read();

            var span = DateTime.UtcNow - now;

            string fileName = category.Dump("c:\\temp");

            Console.WriteLine("'{0}' {1:N0} instances, '{2}' {3:N3} ms", category.Category, 
                count, fileName, span.TotalMilliseconds);
        }

        public static void Test()
        {
            DumpCategory("Memory");
            DumpCategory("Processor");

            DumpCategory("Process");

            DumpCategory(".Net CLR Memory");
            DumpCategory(".Net CLR Exceptions");
            DumpCategory(".Net CLR JIT");
            DumpCategory(".Net CLR LocksAndThreads");
        }
    }
}
