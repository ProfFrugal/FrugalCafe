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
    }
}
