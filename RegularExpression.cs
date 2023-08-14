using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace FrugalCafe
{
    internal static class RegularExpression
    {
        private static readonly Regex pattern = 
            new Regex("[^\\w\\.@\\-_\\+ ]",
            RegexOptions.Compiled, 
            TimeSpan.FromMilliseconds(200.0));

        public static string Sanitize1(this string text)
        {
            return pattern.Replace(text, string.Empty);
        }

        public static string Sanitize(this string text)
        {
            return Regex.Replace(text, "[^\\w\\.@\\-_\\+ ]", string.Empty, 
                RegexOptions.None, TimeSpan.FromMilliseconds(200.0));
        }

        public static void Test()
        {
            string phrase = "A quick brown fox jumps over the lazy dog.";

            PerfTest.MeasurePerf(() => phrase.Sanitize(), "before", 500000);
            PerfTest.MeasurePerf(() => phrase.Sanitize1(), "reuse", 500000);
        }
    }
}
