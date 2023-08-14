using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Concurrent;

namespace FrugalCafe
{
    internal static class LinqIsNotQuick
    {
        public static bool Any1<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw Error.ArgumentNull("source");
            }

            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    return true;
                }
            }

            return false;
        }

        public static bool Any<T>(this ICollection<T> source)
        {
            if (source == null)
            {
                Error.ThrowArgumentNull("source");
            }

            return source.Count != 0;
        }

        public static bool Any<T>(this IReadOnlyCollection<T> source)
        {
            if (source == null)
            {
                Error.ThrowArgumentNull("source");
            }

            return source.Count != 0;
        }

        public static bool Any<T>(this List<T> source)
        {
            return source.Count != 0;
        }

        public static bool Any<T>(this T[] source)
        {
            if (source == null)
            {
                Error.ThrowArgumentNull("source");
            }

            return source.Length != 0;
        }

        public static bool Any(this string source)
        {
            if (source == null)
            {
                Error.ThrowArgumentNull("source");
            }

            return source.Length != 0;
        }

        public static bool Any<T>(this ConcurrentQueue<T> source)
        {
            if (source == null)
            {
                Error.ThrowArgumentNull("source");
            }

            return !source.IsEmpty;
        }

        public static bool SequenceEqual<T>(this ICollection<T> first, ICollection<T> second, IEqualityComparer<T> comparer = null)
        {
            if (object.ReferenceEquals(first, second))
            {
                return true;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            using (IEnumerator<T> e1 = first.GetEnumerator())
            {
                using (IEnumerator<T> e2 = second.GetEnumerator())
                {
                    while (e1.MoveNext())
                    {
                        if (!e2.MoveNext() || !comparer.Equals(e1.Current, e2.Current))
                        {
                            return false;
                        }
                    }

                    if (e2.MoveNext())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool SequenceEqual<T>(this IList<T> one, IList<T> two) where T: IEquatable<T>
        {
            if (object.ReferenceEquals(one, two))
            {
                return true;
            }

            int count = one.Count;

            if (count != two.Count)
            {
                return false;
            }

            for (int i = 0; i < count; i++)
            {
                if (!one[i].Equals(two[i]))
                {
                    return false;
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void TestAny(List<int> source)
        {
            int c = 0;

            for (int i = 0; i < 10; i++)
            {
                bool f = source.Any();

                if (f)
                {
                    c++;
                }

                if (i == 2)
                {
                    Debugger.Break();
                }

                source.Add(i);
            }

            Console.WriteLine(c);
        }

        public static void TestLinq()
        {
            var data = new List<int>();

            var bag = new ConcurrentBag<int>();

            int i = bag.ElementAt(5);

            data.SequenceEqual(data);

            TestAny(data);
        }
    }

    internal static class Error
    {
        public static ArgumentNullException ArgumentNull(string source)
        {
            return new ArgumentNullException(source);
        }

        public static void ThrowArgumentNull(string source)
        {
            throw new ArgumentNullException(source);
        }
    }
}
