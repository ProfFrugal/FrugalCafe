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

namespace FrugalCafe
{
    /// <summary>
    /// Replacement for .Net Framework string.Format call with 0 or 1 string allocation
    /// , optimal for large string generation.
    /// </summary>
    public class StringFormatter : ISimpleStringBuilder
    {
        private static readonly string Pad256 = new string(' ', 256);

        private Substring[] _strings = new Substring[8];
        private int _stringCount;

        public void Clear(bool clearData)
        {
            for (int i = 0; i < _stringCount; i++)
            {
                _strings[i] = default(Substring);
            }

            _stringCount = 0;
        }

        public void Append(string value)
        {
            if ((value != null) && (value.Length != 0))
            {
                this.Add(new Substring(value));
            }
        }

        public void Append(string format, int pos)
        {
            if (_stringCount != 0)
            {
                ref Substring last = ref _strings[_stringCount - 1];

                if (object.ReferenceEquals(last.Text, format) && ((last.Start + last.Length) == pos))
                {
                    last.Expand(1);

                    return;
                }
            }

            this.Add(new Substring(format, pos, 1));
        }

        public void Pad(int repeat)
        {
            while (repeat > 0)
            {
                int len = Math.Min(repeat, StringFormatter.Pad256.Length);

                Add(new Substring(StringFormatter.Pad256, 0, len));

                repeat -= len;
            }
        }

        public unsafe override string ToString()
        {
            if (_stringCount == 0)
            {
                return string.Empty;
            }

            if (_stringCount == 1)
            {
                return _strings[0].ToString();
            }

            int length = 0;

            for (int i = 0; i < _stringCount; i++)
            {
                length += _strings[i].Length;
            }

            string result = new string(char.MinValue, length);

            fixed (char* dst = result) 
            {
                char* d = dst;

                for (int i = 0; i < _stringCount; i++)
                {
                    Substring sub = _strings[i];

                    int len = sub.Length;

                    fixed (char* src = sub.Text)
                    {
                        char* s = src + sub.Start;

                        switch (len)
                        {
                            case 1:
                                d[0] = s[0];
                                break;

                            case 2:
                                d[0] = s[0];
                                d[1] = s[1];
                                break;

                            case 3:
                                d[0] = s[0];
                                d[1] = s[1];
                                d[2] = s[2];
                                break;

                            default:
                                Buffer.MemoryCopy(s, d, len * 2, len * 2);
                                break;
                        }
                    }
                    
                    d += len;
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(Substring str)
        {
            if (_stringCount == _strings.Length)
            {
                Array.Resize(ref _strings, _stringCount * 2);
            }

            _strings[_stringCount++] = str;
        }
    }
}
