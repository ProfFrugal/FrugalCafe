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

using FrugalLib;
using System;

namespace FrugalCafe
{
    public struct Substring : IEquatable<Substring>, IMatcher<string>
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

        /// <summary>
        /// Exactly the hash code as 64-bit .Net Framework.
        /// </summary>
        public unsafe override int GetHashCode()
        {
            fixed (char* src = Text)
            {
                int hash1 = 5381;
                int hash2 = hash1;

                char* s = src + Start;
                char* end = s + Length;

                while (s < end)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ *s++;

                    if (s == end)
                    {
                        break;
                    }

                    hash2 = ((hash2 << 5) + hash2) ^ *s++;
                }

                return hash1 + (hash2 * 1566083941);
            }
        }

        public void Expand(int add)
        {
            this.Length += add;
        }

        public bool Equals(string other)
        {
            if (Length == other.Length)
            {
                return string.CompareOrdinal(Text, Start, other, 0, Length) == 0;
            }

            return false;
        }

        public string ConvertTo()
        {
            return Text.Substring(Start, Length);
        }

        public int Length { get; private set; }

        public int Start { get; }

        public string Text { get; }

        public char this[int index] => Text[Start + index];
    }
}