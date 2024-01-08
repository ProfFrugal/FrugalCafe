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
using System.Runtime.CompilerServices;
using System.Threading;

namespace FrugalCafe
{
    public class SmallObjectPool<T> where T : class, new()
    {
        public static readonly SmallArrayPool<T> Shared = new SmallArrayPool<T>(32);

        private readonly Func<T> _factory;
        private readonly T[] _array;
        private T _singleton;

        public SmallObjectPool(int count, Func<T> factory)
        {
            _factory = factory;

            if (count > 1)
            {
                _array = new T[count - 1];
            }
        }

        public T Rent()
        {
            if (TryRent(ref _singleton, out var result))
            {
                return result;
            }

            if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++) 
                {
                    if (TryRent(ref _array[i], out result))
                    {
                        return result;
                    }
                }
            }

            return _factory();
        }

        public bool Return(T array)
        {
            if (_singleton == null)
            {
                _singleton = array;

                return true;
            }
            else if (_array != null)
            {
                for (int i = 0; i < _array.Length; i++)
                {
                    if (_array[i] == null)
                    {
                        _array[i] = array;

                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryRent(ref T candidate, out T result)
        {
            if (candidate != null)
            {
                result = Interlocked.Exchange(ref candidate, null);

                return result != null;
            }

            result = null;

            return false;
        }
    }
}
