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

        private readonly Dictionary<string, float[]> _processMap = new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
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
