using System;

namespace FrugalCafe.Posts
{
    internal class TestEventSource
    {
        public static void Test()
        {
            var log = FrugalCafeEventSource.Instance;

            Console.WriteLine("{0} {1} {2}", log.Name, log.Guid, log.IsEnabled());

            if (log.IsEnabled())
            {
                log.Start("Test");

                for (int i = 0; i < 10; i++)
                {
                    log.Message(i.ToString());
                }

                log.Stop("Test");
            }
        }
    }
}
