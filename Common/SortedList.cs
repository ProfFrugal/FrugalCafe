using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class SortedList<T>
    {
        private IComparer<T> _comparer;
        private T[] _items;
        private int _count;
        private bool _sorted;

        public SortedList(IComparer<T> comparer = null)
        {
            _comparer = comparer ?? Comparer<T>.Default;
            _sorted = true;
        }

        public void Add(T item)
        {
            if ((_items == null) || (_count == _items.Length))
            {
                int newLength = _count == 0 ? 1 : _count * 2;
                T[] newArray = new T[newLength];

                if (_items != null)
                {
                    _items.CopyTo(newArray, 0);
                }

                _items = newArray;
            }

            _items[_count++] = item;
            _sorted = false;
        }

        public int Count => _count;

        public void Clear(bool clearData = true)
        {
            if ((_count != 0) && clearData)
            {
                Array.Clear(_items, 0, _count);
            }

            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSorted()
        {
            if (!_sorted)
            {
                Sort();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void Sort()
        {
            if (_count > 1)
            {
                Array.Sort<T>(_items, 0, _count, _comparer);
            }

            _sorted = true;
        }

        public class EqualityComparer : IEqualityComparer<SortedList<T>>
        {
            private readonly IEqualityComparer<T> _comparer;

            public EqualityComparer(IEqualityComparer<T> itemComparer = null)
            {
                _comparer = itemComparer ?? EqualityComparer<T>.Default;
            }

            public bool Equals(SortedList<T> x, SortedList<T> y)
            {
                if (object.ReferenceEquals(x, y))
                {
                    return true;
                }

                if ((x == null) || (y == null) || (x.Count != y.Count))
                {
                    return false;
                }

                x.EnsureSorted();
                y.EnsureSorted();

                for (int i = 0; i < x.Count; i++)
                {
                    if (!_comparer.Equals(x._items[i], y._items[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(SortedList<T> obj)
            {
                if (obj == null)
                {
                    return 0;    
                }

                obj.EnsureSorted();

                int hash = 0;

                for (int i = 0; i < obj._count; i++)
                {
                    hash = hash.Combine(_comparer.GetHashCode(obj._items[i]));
                }

                return hash;
            }
        }
    }
}
