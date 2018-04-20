using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;

namespace Ignite
{
    public static class ThinClientPutGetExample
    {
        /// <summary> Cache name. </summary>
        private const string CacheName = "thinClientCache";

        [STAThread]
        public static void Main()
        {
            var cfg = new IgniteClientConfiguration
            {
                Host = "127.0.0.1"
            };

            using (IIgniteClient igniteClient = Ignition.StartClient(cfg))
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache put-get client example started.");

               // ICacheClient<int, Organization> cache = igniteClient.GetOrCreateCache<int, Organization>(CacheName);

               // PutGet(cache);
            }

            Console.WriteLine();
            Console.WriteLine(">>> Example finished, press any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// Execute individual Put and Get.
        /// </summary>
        /// <param name="cache">Cache instance.</param>
        private static void PutGet(ICacheClient<int, Organization> cache)
        {
            // Create new Organization to store in cache.
           Organization org = new Organization(
                "Microsoft",
               new Address("1096 Eddy Street, San Francisco, CA", 94109),
                OrganizationType.Private,
               DateTime.Now
            );

             Put created data entry to cache.
            cache.Put(1, org);

            // Get recently created employee as a strongly-typed fully de-serialized instance.
           Organization orgFromCache = cache.Get(1);

            Console.WriteLine();
            Console.WriteLine(">>> Retrieved organization instance from cache: " + orgFromCache);
        }
    }

