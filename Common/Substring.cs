using System;
using System.Threading;

namespace FrugalCafe
{
    public struct Substring
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

        public override string ToString()
        {
            return _text.Substring(_start, _length);
        }

        public int Length => _length;

        public char this[int index] => _text[_start + index];
    }
}