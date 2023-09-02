using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe.Common
{
    public class SimpleHashSet<T>
    {
        private struct Slot
        {
            internal T value;
            internal int hashCode;
            internal int next;
        }

        IEqualityComparer<T> _comparer;
        int[] _buckets;
        Slot[] _slots;

        int _count;
        int _freeList;
        
        public SimpleHashSet(IEqualityComparer<T> comparer = null)
        {
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _freeList = -1;
        }

        public int Count => _count;

        public int Capacity => (_buckets != null) ? _buckets.Length : 0;

        public bool Add(T value)
        {
            int hashCode = InternalGetHashCode(value);

            if ((_count > 0) && (FindSlot(value, hashCode) >= 0))
            {
                return false;
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
                }

                index = _count;
                _count++;
            }

            int bucket = hashCode % _buckets.Length;

            _slots[index].hashCode = hashCode;
            _slots[index].value = value;
            _slots[index].next = _buckets[bucket] - 1;

            _buckets[bucket] = index + 1;

            return true;
        }

        public bool Contains(T value)
        {
            return (_count > 0) && (FindSlot(value, InternalGetHashCode(value)) >= 0);
        }

        public bool TryGetValue(T value, out T result)
        {
            if (_count > 0)
            {
                int slot = FindSlot(value, InternalGetHashCode(value));

                if (slot >= 0)
                {
                    result = _slots[slot].value;

                    return true;
                }
            }

            result = default(T);
            
            return false;
        }

        public bool Remove(T value)
        {
            if (_count > 0)
            {
                int hashCode = InternalGetHashCode(value);

                int bucket = hashCode % _buckets.Length;
                int last = -1;

                for (int i = _buckets[bucket] - 1; i >= 0; last = i, i = _slots[i].next)
                {
                    if ((_slots[i].hashCode == hashCode) && _comparer.Equals(_slots[i].value, value))
                    {
                        if (last < 0)
                        {
                            _buckets[bucket] = _slots[i].next + 1;
                        }
                        else
                        {
                            _slots[last].next = _slots[i].next;
                        }

                        _slots[i].hashCode = -1;
                        _slots[i].value = default(T);
                        _slots[i].next = _freeList;
                        _freeList = i;

                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindSlot(T value, int hashCode)
        {
            for (int i = _buckets[hashCode % _buckets.Length] - 1; i >= 0; i = _slots[i].next)
            {
                if (_slots[i].hashCode == hashCode && _comparer.Equals(_slots[i].value, value))
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

            int newSize = HashHelpers.GetPrime(_count * 2 + 1, HashHelpers.Primes16);

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
        private int InternalGetHashCode(T value)
        {
            return _comparer.GetHashCode(value) & 0x7FFFFFFF;
        }
    }
}
