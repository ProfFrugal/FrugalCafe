using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FrugalCafe
{
    public class DictionaryArray<K, V>
    {
        private readonly Dictionary<K, V>[] _maps;
        private readonly IEqualityComparer<K> _comparer;

        public DictionaryArray(int partition, IEqualityComparer<K> comparer = null)
        { 
            _maps = new Dictionary<K, V>[partition];
            _comparer = comparer ?? EqualityComparer<K>.Default;
        }

        public V this[K key] 
        { 
            get
            {
                return this.GetPartition(key)[key];
            }

            set
            {
                this.GetPartition(key)[key] = value;
            }
        }

        public void Add(K key, V value) 
        {
            this.GetPartition(key).Add(key, value);
        }

        public bool TryGetValue(K key, out V value)
        {
            return this.GetPartition(key).TryGetValue(key, out value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dictionary<K, V> GetPartition(K key)
        {
            uint index = (uint)key.GetHashCode() % (uint)_maps.Length;

            var p = _maps[index];

            if (p == null)
            {
                p = new Dictionary<K, V>(_comparer);

                _maps[index] = p;
            }

            return p;
        }
    }
}
