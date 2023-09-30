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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class SimpleDictionary<K, V>
    {
        private struct Slot
        {
            internal K key;
            internal V value;
            internal int hashCode;
            internal int next;
        }

        IEqualityComparer<K> _comparer;
        int[] _buckets;
        Slot[] _slots;

        int _count;
        int _freeList;
        
        public SimpleDictionary(IEqualityComparer<K> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<K>.Default;
            _freeList = -1;
        }

        public int Count => _count;

        public int Capacity => (_buckets != null) ? _buckets.Length : 0;

        public void Add(K key, V value)
        {
            TryAdd(key, value, InsertionBehavior.ThrowOnExisting);
        }

        public bool TryAdd(K key, V value, InsertionBehavior whenFound)
        {
            int hashCode = InternalGetHashCode(key);

            int bucket = 0;

            if (_count > 0)
            {
                bucket = hashCode % _buckets.Length;

                for (int i = bucket - 1; i >= 0; i = _slots[i].next)
                {
                    if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].key, key))
                    {
                        if (whenFound == InsertionBehavior.ThrowOnExisting)
                        {
                            throw new ArgumentOutOfRangeException(nameof(key));
                        }
                        else if (whenFound == InsertionBehavior.OverwriteExisting)
                        {
                            _slots[i].value = value;

                            return true;
                        }

                        return false;
                    }
                }
            }

            int index;

            if (_freeList >= 0)
            {
                index = _freeList;
                _freeList = _slots[index].next;
            }
            else
            {
                if ((_slots == null) || (_count == _slots.Length))
                {
                    Resize();
                    bucket = hashCode % _buckets.Length;
                }

                index = _count;
                _count++;
            }

            _slots[index].key = key;
            _slots[index].value = value;
            _slots[index].hashCode = hashCode;
            _slots[index].next = _buckets[bucket] - 1;

            _buckets[bucket] = index + 1;

            return true;
        }

        public bool Contains(K key)
        {
            return (_count > 0) && (FindSlot(key, InternalGetHashCode(key)) >= 0);
        }

        public bool TryGetValue(K key, out V value)
        {
            if (_count > 0)
            {
                int slot = FindSlot(key, InternalGetHashCode(key));

                if (slot >= 0)
                {
                    value = _slots[slot].value;

                    return true;
                }
            }

            value = default(V);
            
            return false;
        }

        public bool Remove(K key)
        {
            if (_count > 0)
            {
                int hashCode = InternalGetHashCode(key);

                int bucket = hashCode % _buckets.Length;
                int last = -1;

                for (int i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].next)
                {
                    if ((_slots[i].hashCode == hashCode) && _comparer.Equals(_slots[i].key, key))
                    {
                        if (last < 0)
                        {
                            _buckets[bucket] = _slots[i].next + 1;
                        }
                        else
                        {
                            _slots[last].next = _slots[i].next;
                        }

                        _slots[i].key = default(K);
                        _slots[i].value = default(V);
                        _slots[i].hashCode = -1;
                        _slots[i].next = _freeList;
                        _freeList = i;

                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindSlot(K key, int hashCode)
        {
            for (int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].next)
            {
                if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].key, key))
                {
                    return i;
                }
            }

            return -1;
        }

        void Resize()
        {
            if (_slots == null)
            {
                _buckets = new int[2];
                _slots = new Slot[2];

                return;
            }

            int newSize = HashHelpers.GetPrime(_count * 2 + 1, HashHelpers.Primes24);

            int[] newBuckets = new int[newSize];
            Slot[] newSlots = new Slot[newSize];
            
            Array.Copy(_slots, 0, newSlots, 0, _count);
            
            for (int i = 0; i < _count; i++)
            {
                int bucket = newSlots[i].hashCode % newSize;
                newSlots[i].next = newBuckets[bucket] - 1;
                newBuckets[bucket] = i + 1;
            }
            
            _buckets = newBuckets;
            _slots = newSlots;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int InternalGetHashCode(K value)
        {
            return _comparer.GetHashCode(value) & 0x7FFFFFFF;
        }
    }
}
