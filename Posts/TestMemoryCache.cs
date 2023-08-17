using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FrugalCafe.Posts
{
    internal class TestMemoryCache
    {
        public static void Test()
        {
            MemoryCache cache = new MemoryCache(new MemoryCacheOptions());

            for (int i = 0; i < 100; i++)
            {
                string key = "key" + i;
                cache.Set(key, "value" + i, DateTimeOffset.UtcNow + TimeSpan.FromMinutes(5));
            }
        }
    }
}