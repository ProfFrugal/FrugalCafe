using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace FrugalCafe
{
    internal static class TestJsonContractResolver
    {
        public static void Test()
        {
            Dictionary<string, string> map = new Dictionary<string, string>();

            map.Add("one", "1");

            PerfTest.MeasurePerf(
                () =>
                {
                    JsonConvert.SerializeObject(map);
                },
                "default", 
                10000);

            PerfTest.MeasurePerf(
                () =>
                {
                    JsonConvert.SerializeObject(map, null, 
                        new JsonSerializerSettings() { 
                            ContractResolver = new DefaultContractResolver() });
                },
                "resolver",
                10000);
        }
    }
}
