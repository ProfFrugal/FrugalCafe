using System;
using System.Threading;

namespace FrugalCafe
{
    public struct Substring : IEquatable<Substring>
    {
        private readonly string _text;
        private readonly int _start;
        private readonly int _length;

        public Substring(string value, int start, int length)
        {
            _text = value;
            _start = start;
            _length = length;
        }

        public Substring(string value)
        {
            _text = value;
            _start = 0;
            _length = value.Length;
        }

        public override string ToString()
        {
            return _text.Substring(_start, _length);
        }

        public bool Equals(Substring other)
        {
            if (_length == other._length)
            {
                return string.CompareOrdinal(_text, _start, other._text, other._start, _length) == 0;
            }

            return false;
        }

        public override bool Equals(object other)
        {
            if (other is Substring ss)
            {
                return this.Equals(ss);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < _length; i++)
            {
                hash = hash * 131 + _text[_start + i];
            }

            return hash;
        }

        public int Length => _length;

        public char this[int index] => _text[_start + index];
    }
}