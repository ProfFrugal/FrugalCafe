using System;

namespace FrugalCafe
{
    public struct Substring : IEquatable<Substring>
    {
        public Substring(string value, int start, int length)
        {
            Text = value;
            Start = start;
            Length = length;
        }

        public Substring(string value)
        {
            Text = value;
            Start = 0;
            Length = value.Length;
        }

        public override string ToString()
        {
            return Text.Substring(Start, Length);
        }

        public bool Equals(Substring other)
        {
            if (Length == other.Length)
            {
                return string.CompareOrdinal(Text, Start, other.Text, other.Start, Length) == 0;
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

            for (int i = 0; i < Length; i++)
            {
                hash = hash * 131 + Text[Start + i];
            }

            return hash;
        }

        public void Expand(int add)
        {
            this.Length += add;
        }

        public int Length { get; private set; }

        public int Start { get; }

        public string Text { get; }

        public char this[int index] => Text[Start + index];
    }
}