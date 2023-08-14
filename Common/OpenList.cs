using System;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class OpenList<T>
    {
        protected T[] _items;
        protected int _count;
        protected int _freeSpace;

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
    }

    public static class ThrowHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void ThrowIndexOutOfRangeException()
        {
            throw new IndexOutOfRangeException();
        }
    }

    public class StringList : OpenList<string>
    {
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
    }
}
