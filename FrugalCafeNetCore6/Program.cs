// See https://aka.ms/new-console-template for more information
using FrugalCafe;
using FrugalCafeNetCore6;
using System.Collections.Concurrent;
using System.Text;

Console.WriteLine("Hello, World!");

// TestAny.Test();

// ConcurrentDictionary<string, int> map = new ConcurrentDictionary<string, int>();

// TestValueStringBuilder.PerfTest();

PerfTest.Start();

int length = 0;

for (int i = 0; i < 100; i++)
{
    length += TestAny.StringBuilderCached().Length;
}

PerfTest.Stop("cached", 100);

Console.WriteLine(length / 100);

