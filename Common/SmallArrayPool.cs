// Copyright (c) 2023 Feng Yuan for https://frugalcafe.beehiiv.com/
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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

                        break;
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

        public void Release(int maxLength)
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
