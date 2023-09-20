using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace HeapWalker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DataTarget target = Microsoft.Diagnostics.Runtime.DataTarget.LoadCrashDump(args[0]);

            var clrs = target.ClrVersions;

            if ((clrs == null) || (clrs.Count == 0))
            {
                Console.WriteLine("ClrVersion not found");

                return;
            }

            ClrInfo clr = target.ClrVersions[0];

            string dac = clr.TryGetDacLocation();

            if (string.IsNullOrEmpty(dac)) 
            {
                dac = "c:\\windows\\Microsoft.NET\\Framework64\\v4.0.30319\\mscordacwks.dll";
            }

            Console.WriteLine("{0} '{1}'", clr.ToString(), dac);

            ClrRuntime runtime = target.CreateRuntime(dac);

            ClrHeap heap = runtime.GetHeap();

            Console.WriteLine("{0} segments", heap.Segments.Count);

            foreach (var segment in heap.Segments) 
            {
                ulong start = segment.Start;

                Console.WriteLine("{0:x}'{1:x8} {2:x} {3:N0} bytes", segment.Start >> 32, (uint)(segment.Start), segment.End, segment.Length);
            }

            Console.WriteLine();

            var wrapper = new NetDbgObj(heap);

            if ((args.Length >= 3) && (args[1] == "gen0"))
            {
                wrapper.WalkGen0(args[2]);
            }
            else if ((args.Length == 2) && (args[1] == "old"))
            {
                Console.WriteLine("Slow original Linq based implementation.");

                wrapper.Print40LargestObjects();
            }
            else
            {
                wrapper.Print40LargestObjectsFixed();
            }
        }

        class Stat
        {
            internal ulong Count;
            internal ulong TotalSize;
            internal ulong Largest;

            public void Add(ulong size)
            {
                Count++;
                TotalSize += size;

                if (size > Largest)
                {
                    Largest = size;
                }
            }

            public void Add(Stat s)
            {
                Count += s.Count;
                TotalSize += s.TotalSize;

                if (s.Largest > Largest)
                {
                    Largest = s.Largest;
                }
            }

            public void Write(string typeName)
            {
                Console.WriteLine("{0,14:N0} {1,12:N0}, {2,14:N0}, {3}", TotalSize, Count, Largest, typeName);
            }
        }

        static DateTime _lastMillion;

        public static void Progress(int count)
        {
            long memory = GC.GetTotalMemory(false);

            DateTime now = DateTime.UtcNow;

            Console.WriteLine(
                "{0:N0} objects scanned, memory: {1:N2} mb, {2:N2} bytes per object, {3:N0} ms", 
                count, 
                memory / 1024.0 / 1024, 
                memory * 1.0 / count,
                (now - _lastMillion).TotalMilliseconds);

            _lastMillion = now;
        }

        class NetDbgObj
        {
            private readonly ClrHeap _heap;

            public NetDbgObj(ClrHeap heap)
            {
                _heap = heap;
            }

            long _alloc;
            DateTime _now;

            public void Pre()
            {
                _alloc = GC.GetAllocatedBytesForCurrentThread();
                _now = DateTime.UtcNow;
                _lastMillion = DateTime.UtcNow;
            }

            public void Post()
            {
                _alloc = GC.GetAllocatedBytesForCurrentThread() - _alloc;

                var span = DateTime.UtcNow - _now;

                Console.WriteLine("Total allocation: {0:N0} bytes {1:N2} s", _alloc, span.TotalSeconds);

            }

            public void Print40LargestObjects()
            {
                Pre();

                try
                {
                    var source = from obj in this.EnumerateHeapObjects()
                                 let type = obj.GetHeapType()
                                 where type != null
                                 group obj by type into g
                                 select new
                                 {
                                     TypeName = g.Key.Name,
                                     NumObjects = g.Count(),
                                     TotalSize = g.Sum((ClrObject o) => (long)g.Key.GetSize(o.GetValue()))
                                 };

                    source = source.OrderByDescending(e => e.TotalSize).Take(40);

                    foreach (var s in source)
                    {
                        Console.WriteLine("{0,14:N0} {1,12:N0}, {2}", s.TotalSize, s.NumObjects, s.TypeName);
                    }
                }
                catch (OutOfMemoryException e)
                {
                    Console.WriteLine(e);
                }

                Post();
            }

            public void WalkGen0(string marker)
            {
                Console.WriteLine("Seaching for {0} in Gen0", marker);

                foreach (ClrSegment seg in _heap.Segments)
                {
                    if (!seg.IsEphemeral) continue;

                    ulong gen0Start = seg.Gen0Start;
                    ulong gen0End = gen0Start + seg.Gen0Length;

                    bool found = false;

                    foreach (ulong addr in seg.EnumerateObjects())
                    {
                        if (addr >= gen0Start && addr <= gen0End)
                        {
                            ClrType type = _heap.GetObjectType(addr);

                            if (type != null)
                            {
                                string name = type.Name;

                                if (name.EndsWith(marker))
                                {
                                    if (found)
                                    {
                                        break;
                                    }

                                    found = true;
                                }

                                if (found)
                                {
                                    ulong size = type.GetSize(addr);

                                    Console.WriteLine("{0:x},{1},\"{2}\"", addr, size, name);
                                }
                            }
                        }
                    }
                }
            }

            public void Print40LargestObjectsFixed()
            {
                Pre();

                Dictionary<ClrType, Stat> stats = new Dictionary<ClrType, Stat>();

                int count = 0;

                foreach (ulong addr in _heap.EnumerateObjects())
                {
                    count++;

                    if (count % (1024 * 1024) == 0)
                    {
                        Progress(count);
                    }

                    ClrType type = _heap.GetObjectType(addr);

                    if (type != null)
                    {
                        ulong size = type.GetSize(addr);

                        if (!stats.TryGetValue(type, out var s))
                        {
                            s = new Stat();
                            stats.Add(type, s);
                        }

                        s.Add(size);
                    }
                }

                Stat total = new Stat();

                foreach (var v in stats.Values)
                {
                    total.Add(v);
                }

                var data = stats.ToArray();

                Array.Sort(data, (x, y) => y.Value.TotalSize.CompareTo(x.Value.TotalSize));

                int len = Math.Min(data.Length, 40);

                Console.WriteLine("{0,14} {1,12}, {2,14}, {3}", "Size", "Count", "Largest", "Type");

                total.Write("Total");

                for (int i = 0; i < len; i++)
                {
                    var s = data[i];

                    s.Value.Write(s.Key.Name);
                }

                Post();
            }

            public IEnumerable<ClrObject> EnumerateHeapObjects(string typeName = null)
            {
                int count = 0;

                using (IEnumerator<ulong> enumerator = EnumerateHeap(typeName).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        count++;

                        if (count % (1024 * 1024) == 0)
                        {
                            Progress(count);
                        }

                        yield return new ClrObject(addr: enumerator.Current, heap: _heap, type: null);
                    }
                }
            }

            public IEnumerable<ulong> EnumerateHeap(string typeName)
            {
                foreach (ulong addr in _heap.EnumerateObjects())
                {
                    ClrType type = _heap.GetObjectType(addr);

                    if (type != null && (string.IsNullOrEmpty(typeName) || type.Name == typeName))
                    {
                        yield return addr;
                    }
                }
            }

            public class ClrObject : DynamicObject
            {
                private ulong m_addr;

                private bool m_inner;

                private ClrType m_type;

                private ClrHeap m_heap;

                private int m_len = -1;

                public ulong GetValue()
                {
                    return m_addr;
                }

                public ClrType GetHeapType() => m_type;

                public ClrObject(ClrHeap heap, ClrType type, ulong addr)
                {
                    m_addr = addr;
                    m_heap = heap;
                    if (addr != 0)
                    {
                        ClrType objectType = heap.GetObjectType(addr);
                        if (objectType != null)
                        {
                            type = objectType;
                        }
                    }
                    m_type = type;
                }
            }
        }
    }
}
