using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class SmallDictionary<K, V> where K : IEquatable<K>
    {
        private struct Slot
        {
            internal K key;
            internal V value;
            internal short next;
        }

        short[] _buckets;
        Slot[] _slots;
        ulong _fastModMultiplier;

        int _count;
        
        public SmallDictionary(int capacity = 0)
        {
            if (capacity > 0)
            {
                int size = HashHelpers.GetPrime(capacity, HashHelpers.Primes16);
                
                _buckets = new short[size];
                _slots = new Slot[size];
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
            }
        }

        public int Count => _count;

        public int Capacity => (_buckets != null) ? _buckets.Length : 0;

        public V this[K key]
        {
            get
            {
                if (_count > 0)
                {
                    int slot = FindSlot(key, InternalGetHashCode(key));

                    if (slot >= 0)
                    {
                        return _slots[slot].value;
                    }
                }

                throw new KeyNotFoundException();
            }

            set
            {
                TryAdd(key, value, InsertionBehavior.OverwriteExisting);
            }
        }
        public void Add(K key, V value)
        {
            TryAdd(key, value, InsertionBehavior.ThrowOnExisting);
        }

        public bool TryAdd(K key, V value, InsertionBehavior whenFound)
        {
            uint hashCode = InternalGetHashCode(key);

            int bucket = GetBucket(hashCode);

            if (_count > 0)
            {
                for (int i = bucket - 1; i >= 0; i = _slots[i].next)
                { 
                    if (_slots[i].key.Equals(value))
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

            if ((_slots == null) || (_count == _slots.Length))
            {
                Resize();
                bucket = GetBucket(hashCode);
            }

            int index = _count;
            _count++;

            _slots[index].key = key;
            _slots[index].value = value;
            _slots[index].next = (short)(_buckets[bucket] - 1);

            _buckets[bucket] = (short)(index + 1);

            return true;
        }

        public bool ContainsKey(K value)
        {
            return (_count > 0) && (FindSlot(value, InternalGetHashCode(value)) >= 0);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int FindSlot(K value, uint hashCode)
        {
            for (int i = GetBucket(hashCode) - 1; i >= 0; i = _slots[i].next)
            {
                if (_slots[i].key.Equals(value))
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
                _buckets = new short[2];
                _slots = new Slot[2];
                _fastModMultiplier = HashHelpers.GetFastModMultiplier(2);
                return;
            }

            int newSize = HashHelpers.GetPrime(_count * 2 + 1, HashHelpers.Primes16);

            Slot[] newSlots = new Slot[newSize];
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);

            Array.Copy(_slots, 0, newSlots, 0, _count);

            _buckets = new short[newSize];

            for (int i = 0; i < _count; i++)
            {
                int bucket = GetBucket(InternalGetHashCode(newSlots[i].key));
                newSlots[i].next = (short)(_buckets[bucket] - 1);
                _buckets[bucket] = (short)(i + 1);
            }
            
            _slots = newSlots;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBucket(uint hashCode)
        {
            return _buckets[HashHelpers.FastMod(hashCode, (uint)_buckets.Length, _fastModMultiplier)];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint InternalGetHashCode(K value)
        {
            return (uint)value.GetHashCode();
        }
    }
}
