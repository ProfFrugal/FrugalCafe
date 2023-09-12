using System;

namespace FrugalCafe
{
    internal class BillionObjects
    {
        public static object Million()
        {
            object[] holder = new object[1000 * 1000];

            for (int i = 0; i < holder.Length; i++)
            {
                holder[i] = i;
            }

            return holder;
        }

        public static void Test()
        {
            int count = 500;

            object[] holder = new object[count];

            for (int i = 0; i < holder.Length; i++)
            {
                holder[i] = Million();
            }

            Console.WriteLine(holder);
            Console.WriteLine("{0:N0} million objects allocated, memory usage:  {1:N0} mb", count, GC.GetTotalMemory(false) / 1024 / 1024);
            Console.ReadLine();
        }
    }
}
