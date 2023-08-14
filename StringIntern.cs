using System;

namespace FrugalCafe
{
    internal static class StringHelpers
    {
        public static string Intern(string str)
        {
            if (str != null)
            {
                return string.Intern(str);
            }

            return str;
        }

        public static void Test(int count)
        {
            int totalLength = 0;

            for (int i = 0; i < count; i++)
            {
                string s = i.ToString();

                s = Intern(s);

                totalLength += s.Length;
            }

            Console.WriteLine("{0:N0} characters", totalLength);
        }
    }
}
