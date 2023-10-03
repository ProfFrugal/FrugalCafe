using System;
using System.Collections.Generic;
using System.Text;

namespace FrugalCafe
{
    public static class StringExtensions
    {
        public static string FrugalFormat(this string format, object arg0)
        {
            var builder = StringBuilderExtensions.AcquireBuilder();

            builder.AppendFormat(format, arg0);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, object arg0, object arg1)
        {
            var builder = StringBuilderExtensions.AcquireBuilder();

            builder.AppendFormat(format, arg0, arg1);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, object arg0, object arg1, object arg2)
        {
            var builder = StringBuilderExtensions.AcquireBuilder();

            builder.AppendFormat(format, arg0, arg1, arg2);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, params object[] args)
        {
            var builder = StringBuilderExtensions.AcquireBuilder();

            builder.AppendFormat(format, args);

            return builder.ToStringAndRelease();
        }


        public unsafe static bool EqualsHelper64(char* a, char* b, int length)
        {
            while (length >= 12)
            {
                if (*(long*)a != *(long*)b)
                {
                    return false;
                }

                if (*(long*)(a + 4) != *(long*)(b + 4))
                {
                    return false;
                }

                if (*(long*)(a + 8) != *(long*)(b + 8))
                {
                    return false;
                }

                a += 12;
                b += 12;
                length -= 12;
            }

            while (length > 0)
            {
                if (*a != *b)
                {
                    return false;
                }

                a++;
                b++;
                length--;
            }

            return true;
        }
    }
}
