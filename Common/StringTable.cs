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
using System.Text;

namespace FrugalCafe
{
    public class StringTable
    {
        public static readonly StringTable Shared = new StringTable(62851);

        private static readonly string[] Latin1Strings;

        private readonly string[] _cachedStrings;

        static StringTable()
        {
            StringTable.Latin1Strings = new string[256];

            for (int i = 0; i < 256; i++)
            {
                StringTable.Latin1Strings[i] = ((char)i).ToString();
            }
        }

        public StringTable(int stringCount)
        {
            _cachedStrings = new string[stringCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string Intern(string text)
        {
            if (text == null)
            {
                return null;
            }

            fixed (char* p = text)
            {
                return Intern(p, text.Length, text);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string Intern(string text, int start, int length)
        {
            if (text == null)
            {
                return null;    
            }

            if (length == 0)
            {
                return string.Empty;
            }

            fixed (char * p = text)
            {
                if ((start != 0) || (text.Length != length))
                {
                    text = null;
                }

                return Intern(p + start, length, text);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Intern(char[] text)
        {
            if (text == null)
            {
                return null;
            }

            return Intern(text, 0, text.Length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string Intern(char[] text, int start, int length)
        {
            if (text == null)
            {
                return null;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            fixed (char* p = text)
            {
                return Intern(p + start, length);
            }
        }

        public unsafe string Intern(char* text, int length, string fullText = null)
        {
            if (text == null)
            {
                return null;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            if ((length == 1) && (text[0] < StringTable.Latin1Strings.Length))
            {
                return StringTable.Latin1Strings[text[0]];
            }

            uint slot = StringTable.GetHashCode(text, length) % (uint)_cachedStrings.Length;

            string cached = _cachedStrings[slot];

            if ((cached != null) && (cached.Length == length))
            {
                fixed (char * p = cached)
                {
                    if (StringExtensions.EqualsHelper64(p, text, length))
                    {
                        return cached;
                    }
                }
            }

            cached = fullText ?? new string(text, 0, length);

            _cachedStrings[slot] = cached;

            return cached;
        }

        public static unsafe uint GetHashCode(char* text, int length)
        {
            uint hash = 31;

            char* end = text + length;

            while (text < end)
            {
                hash = hash.Combine((uint)*text++);
            }

            return hash;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string InternUtf8(byte[] text, int length)
        {
            fixed (byte* p = text)
            {
                return InternUtf8(p, length);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe string InternUtf8(byte[] text, int start, int length)
        {
            fixed (byte * p = text)
            {
                return InternUtf8(p + start, length);
            }
        }

        public unsafe string InternUtf8(byte* text, int length)
        {
            if (text == null)
            {
                return null;
            }

            if (length == 0)
            {
                return string.Empty;
            }

            if (length == 1)
            {
                return StringTable.Latin1Strings[text[0]];
            }

            uint hash = 31;

            byte* end = text + length;

            while (text < end)
            {
                byte ch = *text++;

                if (ch >= 128)
                {
                    return Encoding.UTF8.GetString(text, length);
                }

                hash = hash.Combine(ch);
            }

            uint slot = hash % (uint)_cachedStrings.Length;

            string cached = _cachedStrings[slot];

            if ((cached != null) && (cached.Length == length))
            {
                fixed (char* p = cached)
                {
                    bool same = true;

                    for (int i = 0; i < length; i++)
                    {
                        if (p[i] != cached[i])
                        {
                            same = false;
                            break;
                        }
                    }

                    if (same)
                    {
                        return cached;
                    }
                }
            }

            cached = Encoding.UTF8.GetString(text, length);

            _cachedStrings[slot] = cached;

            return cached;
        }
    }
}
