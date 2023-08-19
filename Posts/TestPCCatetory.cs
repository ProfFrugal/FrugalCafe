using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    internal class TestPCCatetory
    {
        public static void Test()
        {
            var process = new PCCategory("Process");

            int c2 = process.Read();
            process.Dump("c:\\temp");

            Console.WriteLine("{0} {1:N0} instances", process.Category, c2);

            var clrMemory = new PCCategory(".Net CLR Memory");

            int c1 = clrMemory.Read();
            clrMemory.Dump("c:\\temp");

            Console.WriteLine("{0} {1:N0} instances", clrMemory.Category, c1);
        }
    }
}
