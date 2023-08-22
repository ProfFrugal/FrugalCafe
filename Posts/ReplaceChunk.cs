using System;
using System.Collections.Generic;

namespace FrugalCafe
{
    internal class ReplaceChunk
    {
        public static void Test()
        {
            List<string> names = new List<string>() { "Milan", "Pavle", "Stefan", "Umber", "Nabi", "Steve" };

            for (int i = 0; i < names.Count; i += 2)
            {
                string result = names[i];

                if ((i + 1) < names.Count)
                {
                    result = result + "," + names[i + 1];
                }

                Console.WriteLine(result);
            }
        }
    }
}
