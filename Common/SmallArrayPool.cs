using System;
using System.Threading;

namespace FrugalCafe
{
    public class SmallArrayPool<T>
    {
        public static readonly SmallArrayPool<T> Shared = new SmallArrayPool<T>(32);

        private readonly T[][] _array;
        private T[] _singleton;

        public SmallArrayPool(int count)
        {
            if (count > 1)
            {
                _array = new T[count - 1][];
            }
        }

        public T[] Rent(int minimumSize)
        {
            if (_singleton != null)
            {
                var candidate = _singleton;

                if ((candidate != null) && (candidate.Length >= minimumSize))
                {
                    candidate = Interlocked.Exchange(ref _singleton, null);

                    if ((candidate != null) && (candidate.Length >= minimumSize))
                    {
                        return candidate;
                    }
                }
            }

            if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++) 
                {
                    var candidate = _array[i];

                    if ((candidate != null) && (candidate.Length >= minimumSize))
                    {
                        candidate = Interlocked.Exchange(ref _array[i], null);

                        if ((candidate != null) && (candidate.Length >= minimumSize))
                        {
                            return candidate;
                        }
                    }
                }
            }

            return new T[minimumSize];
        }

        public void Return(T[] array)
        {
            int size = GetLength(array);

            if (GetLength(_singleton) < size)
            {
                _singleton = array;
            }
            else if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++)
                {
                    if (GetLength(_array[i]) < size)
                    {
                        _array[i] = array;
                    }
                }
            }
        }

        public int GetTotalLength(out int maxLength)
        {
            int totalLength = GetLength(_singleton);

            maxLength = totalLength;

            if (_array != null)
            {
                foreach (var a in _array)
                {
                    int l = GetLength(a);

                    if (l > maxLength)
                    {
                        maxLength = l;
                    }

                    totalLength += GetLength(a);
                }
            }

            return totalLength;
        }

        public void Clear(int maxLength)
        {
            if (GetLength(_singleton) > maxLength)
            {
                _singleton = null;
            }

            if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++)
                { 
                    if (GetLength(_array[i]) > maxLength)
                    {
                        _array[i] = null;
                    }
                }
            }
        }

        private static int GetLength(T[] array)
        {
            return (array != null) ? array.Length : 0;
        }
    }
}
