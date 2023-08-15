using System;

namespace FrugalCafe
{
    public struct ComboKey<T1, T2> : IEquatable<ComboKey<T1, T2>> 
        where T1 : IEquatable<T1> where T2 : IEquatable<T2>
    {
        private readonly T1 _v1;
        private readonly T2 _v2;

        public ComboKey(T1 v1, T2 v2)
        {
            _v1 = v1;
            _v2 = v2;   
        }

        public bool Equals(ComboKey<T1, T2> other)
        {
            return _v1.Equals(other._v1) && _v2.Equals(other._v2);
        }

        public override int GetHashCode()
        {
            return _v1.GetHashCode().Combine(_v2.GetHashCode());
        }
    }

    public struct ComboKey<T1, T2, T3> : IEquatable<ComboKey<T1, T2, T3>> 
        where T1 : IEquatable<T1> where T2 : IEquatable<T2> where T3 : IEquatable<T3>
    {
        private readonly T1 _v1;
        private readonly T2 _v2;
        private readonly T3 _v3;

        public ComboKey(T1 v1, T2 v2, T3 v3)
        {
            _v1 = v1;
            _v2 = v2;
            _v3 = v3;
        }

        public bool Equals(ComboKey<T1, T2, T3> other)
        {
            return _v1.Equals(other._v1) && _v2.Equals(other._v2) && _v3.Equals(other._v3);
        }

        public override int GetHashCode()
        {
            return _v1.GetHashCode().Combine(_v2.GetHashCode()).Combine(_v3.GetHashCode());
        }
    }
}
