using Microsoft.Extensions.ObjectPool;
using System.Text;

namespace FrugalCafe
{
    public static class StringBuilderExtensions
    {
        public readonly static ObjectPool<StringBuilder> BuilderPool = 
            (new DefaultObjectPoolProvider()).CreateStringBuilderPool(
                initialCapacity:256, maximumRetainedCapacity: 64 * 1024);

        public static StringBuilder AcquireBuilder()
        {
            return BuilderPool.Get();
        }

        public static void Release(this StringBuilder builder)
        {
            if (builder != null)
            {
                BuilderPool.Return(builder);
            }
        }

        public static string ToStringAndRelease(this StringBuilder builder) 
        { 
            if (builder == null)
            {
                return null;
            }

            string result = builder.ToString();

            BuilderPool.Return(builder);

            return result;
        }

        public static string FrugalFormat(this string format, object arg0)
        {
            var builder = AcquireBuilder();

            builder.AppendFormat(format, arg0);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, object arg0, object arg1)
        {
            var builder = AcquireBuilder();

            builder.AppendFormat(format, arg0, arg1);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, object arg0, object arg1, object arg2)
        {
            var builder = AcquireBuilder();

            builder.AppendFormat(format, arg0, arg1, arg2);

            return builder.ToStringAndRelease();
        }

        public static string FrugalFormat(this string format, params object[] args)
        {
            var builder = AcquireBuilder();

            builder.AppendFormat(format, args);

            return builder.ToStringAndRelease();
        }
    }
}
