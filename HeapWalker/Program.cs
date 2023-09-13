﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            ClrRuntime runtime = target.CreateRuntime(clr.TryGetDacLocation());

            ClrHeap heap = runtime.GetHeap();

            Console.WriteLine("{0}", heap.Segments.Count);

            var wrapper = new NetDbgObj(heap);

        //  wrapper.Print40LargestObjectsFixed();
            wrapper.Print40LargestObjects();
        }

        class Stat
        {
            internal ulong count;
            internal ulong size;
        }

        class NetDbgObj
        {
            private readonly ClrHeap _heap;

            public NetDbgObj(ClrHeap heap)
            {
                _heap = heap;
            }

            public void Print40LargestObjects()
            {
                long alloc = GC.GetAllocatedBytesForCurrentThread();

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

                alloc = GC.GetAllocatedBytesForCurrentThread() - alloc;

                Console.WriteLine("Total allocation: {0:N0}", alloc);
            }

            public void Print40LargestObjectsFixed()
            {
                long alloc = GC.GetAllocatedBytesForCurrentThread();

                Dictionary<ClrType, Stat> stats = new Dictionary<ClrType, Stat>();

                foreach (ulong addr in _heap.EnumerateObjects())
                {
                    ClrType type = _heap.GetObjectType(addr);

                    if (type != null)
                    {
                        ulong size = type.GetSize(addr);

                        if (!stats.TryGetValue(type, out var s))
                        {
                            s = new Stat();
                            stats.Add(type, s);
                        }

                        s.count++;
                        s.size += size;
                    }
                }

                var data = stats.ToArray();

                Array.Sort(data, (x, y) => y.Value.size.CompareTo(x.Value.size));

                int len = Math.Min(data.Length, 40);

                for (int i = 0; i < len; i++)
                {
                    var s = data[i];

                    Console.WriteLine("{0,14:N0} {1,12:N0}, {2}", s.Value.size, s.Value.count, s.Key.Name);
                }

                alloc = GC.GetAllocatedBytesForCurrentThread() - alloc;

                Console.WriteLine("Total allocation: {0:N0}", alloc);
            }

            public IEnumerable<ClrObject> EnumerateHeapObjects(string typeName = null)
            {
                using (IEnumerator<ulong> enumerator = EnumerateHeap(typeName).GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
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
