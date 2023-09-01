using System;
using System.Collections.Generic;

namespace FrugalCafe
{
    public static class LinqReplacements
    {
        public static void InplaceOrderBy<T>(this List<T> list, IComparer<T> comparer = null)
        {
            list.Sort(comparer);
        }

        public static void InplaceOrderByDescending<T>(this List<T> list, IComparer<T> comparer = null)
        {
            list.Sort(new DescendingComparer<T>(comparer));
        }

        public class DescendingComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> _comparer;

            public DescendingComparer(IComparer<T> comparer = null)
            {
                _comparer = comparer ?? Comparer<T>.Default;
            }

            public int Compare(T x, T y)
            {
                return _comparer.Compare(y, x);
            }
        }

        public static void InplaceOrderBy<T, K>(this List<T> list, Func<T, K> selector, IComparer<K> comparer = null)
        {
            if (list.Count <= 1)
            {
                return;
            }

            list.Sort(new FuncComparer<T, K>(selector, comparer));
        }

        public static void InplaceOrderByDescending<T, K>(this List<T> list, Func<T, K> selector, IComparer<K> comparer = null)
        {
            if (list.Count <= 1)
            {
                return;
            }

            list.Sort(new DescendingFuncComparer<T, K>(selector, comparer));
        }

        public class FuncComparer<T, K> : IComparer<T>
        {
            private readonly Func<T, K> _selector;
            private readonly IComparer<K> _comparer;

            public FuncComparer(Func<T, K> selector, IComparer<K> comparer = null) 
            { 
                _selector = selector;
                _comparer = comparer ?? Comparer<K>.Default;
            }
            
            public int Compare(T x, T y)
            {
                return _comparer.Compare(_selector(x), _selector(y));
            }
        }

        public class DescendingFuncComparer<T, K> : IComparer<T>
        {
            private readonly Func<T, K> _selector;
            private readonly IComparer<K> _comparer;

            public DescendingFuncComparer(Func<T, K> selector, IComparer<K> comparer = null)
            {
                _selector = selector;
                _comparer = comparer ?? Comparer<K>.Default;
            }

            public int Compare(T x, T y)
            {
                return _comparer.Compare(_selector(y), _selector(x));
            }
        }
    }
}
