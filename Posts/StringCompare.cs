using System;
using System.Linq;

namespace FrugalCafe
{
    internal static class StringCompare
    {
        public static void PerfTest()
        {
            string phrase = "A quick brown fox jumps over the lazy dog.";

            string[] words = phrase.Split(' ');

            Array.Sort<string>(words);

            int count = 4000 * 1000;

            int length = 0;

            FrugalCafe.PerfTest.MeasurePerf(
                () =>
                {
                    var w = (string[])words.Clone();
                    length = w.OrderBy(k => k).Count();
                },
                "OrderBy string.CompareTo",
                count);

            FrugalCafe.PerfTest.MeasurePerf(
                () => 
                {
                    var w = (string[])words.Clone();
                    Array.Sort<string>(w);
                    length = w.Length;
                }, 
                "Array.Sort string.CompareTo", 
                count);

            FrugalCafe.PerfTest.MeasurePerf(
                () =>
                {
                    var w = (string[])words.Clone();
                    length = w.OrderBy(k => k, StringComparer.Ordinal).Count();
                },
                "OrderBy StringComparer.Ordinal",
                count);

            FrugalCafe.PerfTest.MeasurePerf(
                () =>
                {
                    var w = (string[])words.Clone();
                    Array.Sort<string>(w, StringComparer.Ordinal);
                    length = w.Length;
                }, 
                "Array.Sort StringCompare.Ordinal", 
                count);

            FrugalCafe.PerfTest.MeasurePerf(
                () =>
                {
                    var w = (string[])words.Clone();
                    QuickSort(w, 0, w.Length - 1);
                    length = w.Length;
                },
                "DIY QuickSort",
                count);
        }

        public static void QuickSort(string[] data, int left, int right)
        {
            do
            {
                int i = left;
                int j = right;
                string x = data[i + ((j - i) >> 1)];
                
                do
                {
                    while (i < data.Length && string.CompareOrdinal(x, data[i]) > 0) 
                    { 
                        i++; 
                    }

                    while (j >= 0 && string.CompareOrdinal(x, data[j]) < 0)
                    {
                        j--;
                    }

                    if (i > j)
                    {
                        break;
                    }

                    if (i < j)
                    {
                        string temp = data[i];
                        data[i] = data[j];
                        data[j] = temp;
                    }

                    i++;
                    j--;
                } 
                while (i <= j);

                if (j - left <= right - i)
                {
                    if (left < j)
                    {
                        QuickSort(data, left, j);
                    }
                    
                    left = i;
                }
                else
                {
                    if (i < right) 
                    { 
                        QuickSort(data, i, right); 
                    }
                    
                    right = j;
                }
            } 
            while (left < right);
        }
    }
}
