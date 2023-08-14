using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FrugalCafe
{
    internal static class NSquare
    {
        public static string StringConcat(int n)
        {
            string result = string.Empty;

            for (int i = 0; i < n; i++)
            {
                result += i.ToString();
            }

            return result;
        }

        public static string StringAppendFormat(int n)
        {
            string result = string.Empty;

            for (int i = 0; i < n; i++)
            {
                result = string.Format("{0}{1}", result, i);
            }

            return result;
        }

        public static string StringBuilderIndex(int n)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                builder.Append((char)i);
            }

            for (int i = 0; i < n; i++)
            {
                builder[i] = char.ToLower(builder[i]);
            }

            return builder.ToString();
        }

        public static int ElementAt(int n)
        {
            var map = new Dictionary<int, int>(n);

            for (int i = 0; i < n; i++)
            {
                map[i] = i * i;
            }

            int sum = 0;

            for (int i = 0; i < n; i++)
            {
                sum += map.Values.ElementAt(i);
            }

            return sum;
        }

        public static void ListRemoveAt(int n)
        {
            var list = new List<int>(n);

            for (int i = 0; i < n; i++)
            {
                list.Add(i);
            }

            for (int i = 0; i < n; i++)
            {
                list.RemoveAt(0);
            }
        }
    }
}
