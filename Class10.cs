using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ignite
{
    class Cars
    {
        public string Model { get; set; }
        public int Power { get; set; }
        public override string ToString() => $"Model: {Model}, Power: {Power} hp";

        static void Main()
        {
            using (var ignite = Ignition.Start())
            {
                ICache<int, Car> cache = ignite.GetOrCreateCache<int, Car>("cars");
                cache[1] = new Car { Model = "Pagani Zonda R", Power = 740 };
                cache[2] = new Car { Model = "Bmw", Power = 940 };

                foreach (ICacheEntry<int, Car> entry in cache)
                    Console.WriteLine(entry.Key + " " + entry.Value);

                Console.ReadKey();
            }
        }
    }
}
