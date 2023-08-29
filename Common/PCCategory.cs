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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace FrugalCafe
{
    public class PCCategory
    {
        private readonly PerformanceCounterCategory _cat;

        private readonly Dictionary<string, float[]> _processMap = 
            new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
        private string[] _counters;

        public PCCategory(string category)
        {
            Category = category;
            _cat = new PerformanceCounterCategory(category);
        }

        public string Category { get; }

        public int Read()
        {
            _processMap.Clear();

            InstanceDataCollectionCollection collection = _cat.ReadCategory();

            ICollection values = collection.Values;

            if ((_counters == null) || (_counters.Length != values.Count))
            {
                _counters = new string[values.Count];
            }

            int index = 0;

            int count = 0;

            foreach (InstanceDataCollection v in values)
            {
                _counters[index] = v.CounterName;

                foreach (InstanceData d in v.Values)
                {
                    if (!_processMap.TryGetValue(d.InstanceName, out var readings))
                    {
                        readings = new float[values.Count];

                        _processMap.Add(d.InstanceName, readings);
                    }

                    count++;

                    readings[index] = CounterSample.Calculate(CounterSample.Empty, d.Sample);
                }

                index++;
            }

            return count;
        }

        public string Dump(string directory)
        { 
            string fileName = directory + "\\" + Category.Replace(" ", string.Empty) + ".csv";

            using (var writer = new StreamWriter(fileName))
            {
                writer.Write("Process");

                foreach (string c in _counters)
                {
                    writer.Write(',');
                    writer.Write('"');
                    writer.Write(c);
                    writer.Write('"');
                }

                writer.WriteLine();

                foreach (var kv in _processMap)
                {
                    writer.Write('"');
                    writer.Write(kv.Key);
                    writer.Write('"');

                    var list = kv.Value;

                    for (int i = 0; i < list.Length; i++)
                    {
                        writer.Write(',');

                        if (list[i] != float.MinValue)
                        {
                            writer.Write(list[i]);
                        }
                    }

                    writer.WriteLine();
                }
            }

            return fileName;
        }
    }
}
