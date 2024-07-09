using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe.YouTube
{
    internal class String1
    {
        public static void Test()
        {
            System.Diagnostics.Debugger.Break();

            string empty = string.Empty;
            string f = 'F'.ToString();
            string c = 'C'.ToString();
            string fc = f + c;
            string frugal = "Frugal";

            Console.WriteLine(fc);
            Console.WriteLine(frugal);
        }
    }
}
