using FrugalCafe.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace FrugalCafe
{
    public class OpenList<T>
    {
        protected T[] _items;
        protected int _count;
        protected int _freeSpace;

        public OpenList()
        {
        }

        public OpenList(ICollection<T> data)
        {
            _count = data.Count;
            _items = new T[_count];
            data.CopyTo(_items, 0 );
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
        }

        public ref T GetRef(int index)
        {
            if ((uint)index >= _count)
                ThrowHelper.ThrowIndexOutOfRangeException();

            return ref _items[index];
        }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= _count)
                    ThrowHelper.ThrowIndexOutOfRangeException();
                
                return _items[index];
            }
            set
            {
                if ((uint)index >= _count)
                    ThrowHelper.ThrowIndexOutOfRangeException();
                
                _items[index] = value;
            }
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

        public void Sort(IComparer<T> comparer = null)
        {
            if (_count >= 1)
            {
                Array.Sort(_items, 0, _count, comparer);
            }
        }

        public void Sort<K>(Func<T, K> selector, IComparer<K> comparer = null)
        {
            if (_count >= 1)
            {
                K[] keys = SingletonArrayPool<K>.Rent(_count);

                for (int i = 0; i < _count; i++)
                {
                    keys[i] = selector(_items[i]);
                }

                Array.Sort(keys, _items, 0, _count, comparer);

                SingletonArrayPool<K>.Release(keys);
            }
        }

        public void SortDescending<K>(Func<T, K> selector, IComparer<K> comparer = null)
        {
            if (_count >= 1)
            {
                K[] keys = SingletonArrayPool<K>.Rent(_count);

                for (int i = 0; i < _count; i++)
                {
                    keys[i] = selector(_items[i]);
                }

                Array.Sort(keys, _items, 0, _count, new LinqReplacements.DescendingComparer<K>(comparer));

                SingletonArrayPool<K>.Release(keys);
            }
        }

        public void Reverse()
        {
            for (int i = 0; i < _count / 2; i++)
            {
                T t = _items[i];

                _items[i] = _items[_count - 1 - i];
                _items[_count - 1 - i] = t;
            }
        }
    }

    public class StringList : OpenList<string>
    {
        private static StringList reusedList;

        public static StringList Rent()
        {
            var result = Interlocked.Exchange(ref StringList.reusedList, null);

            if (result == null)
            {
                result = new StringList();
            }
            else
            {
                result.Clear();
            }

            return result;
        }

        public static void Return(StringList list)
        {
            StringList.reusedList = list;
        }

        public override string ToString() 
        { 
            switch (_count)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return _items[0];

                case 2:
                    return _items[0] + _items[1];

                case 3:
                    return _items[0] + _items[1] + _items[2];

                case 4:
                    return _items[0] + _items[1] + _items[2] + _items[3];

                default:
                    return string.Join(string.Empty, _items, 0, _count);
            }
        }

        public string ReverseConcat()
        {
            switch (_count)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return _items[0];

                case 2:
                    return _items[1] + _items[0];

                case 3:
                    return _items[2] + _items[1] + _items[1];

                case 4:
                    return _items[3] + _items[2] + _items[1] + _items[0];

                default:
                    base.Reverse();
                    return string.Join(string.Empty, _items, 0, _count);
            }
        }
    }
}
