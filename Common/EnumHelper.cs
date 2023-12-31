﻿// Copyright (c) 2023 Feng Yuan for https://frugalcafe.beehiiv.com/
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
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Threading;

namespace FrugalCafe
{
    public class EnumHelper<T> where T: struct
    {
        private const int MaxFastNameCount = 4096;

        public readonly static EnumHelper<T> Instance = new EnumHelper<T>();

        private readonly Func<T, ulong> _getUInt64;

        private readonly string[] _fastNames;

        private ConcurrentDictionary<T, string> _overFlowCache;

        private int _overFlowCount;

        public EnumHelper()
        {
            Type type = typeof(T);

            T[] values = (T[])Enum.GetValues(type);
            string[] names = Enum.GetNames(type);

            _getUInt64 = GenerateGetLong();

            ulong max = _getUInt64(values[values.Length - 1]);

            if (type.IsDefined(typeof(FlagsAttribute), false))
            {
                max = max * 2 + 1;
            }

            max = Math.Min(max, MaxFastNameCount);

            _fastNames = new string[max + 1];

            for (int i = 0; i < values.Length; i++)
            {
                ulong index = _getUInt64(values[i]);

                if (index <= max)
                {
                    _fastNames[index] = names[i];
                }
            }
        }

        public string FrugalToString(T value)
        {
            ulong index = _getUInt64(value);

            string result;

            if (index < (uint)_fastNames.Length) 
            { 
                result = _fastNames[index];

                if (result == null)
                {
                    result = value.ToString();

                    _fastNames[index] = result;
                }

                return result;
            }

            if (_overFlowCache == null)
            {
                _overFlowCache = new ConcurrentDictionary<T, string>();
            }

            if (_overFlowCache.TryGetValue(value, out result)) 
            {
                return result;
            }

            result = value.ToString();

            if (_overFlowCache.TryAdd(value, result))
            {
                Interlocked.Increment(ref _overFlowCount);

                if (_overFlowCount > MaxFastNameCount * 16)
                {
                    _overFlowCache.Clear();
                    _overFlowCount = 0;
                }
            }

            return result;
        }

        public static Func<T, ulong> GenerateGetLong()
        {
            var method = new DynamicMethod(typeof(T) + "ToUInt64", typeof(ulong), new[] { typeof(T) });
            
            ILGenerator il = method.GetILGenerator();
            
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Conv_U8);
            il.Emit(OpCodes.Ret);
            
            return (Func<T, ulong>)method.CreateDelegate(typeof(Func<T, ulong>));
        }
    }
}
