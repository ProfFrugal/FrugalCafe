using System;

namespace FrugalCafe
{
	internal static class MemoryCacheLeak
	{
		public static void Test()
		{
			int count = 1000 * 100;

			for (int i = 0; i < count; i++)
			{
				var cache = new System.Runtime.Caching.MemoryCache("FrugalCache");

				if (i % 1000 == 0)
				{
					Console.Write('.');
				}
			}

			Console.WriteLine("press enter to quit.");
			Console.ReadLine();
		}
	}
}
