using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IgniteNet2
{
    class PersistenceEnabled
    {

        public static void Main()
        {
            var cfg = new IgniteConfiguration
            {
                        DataStorageConfiguration = new DataStorageConfiguration
                        {
                            DefaultDataRegionConfiguration = new DataRegionConfiguration
                            {
                                Name = "defaultRegion",
                                PersistenceEnabled = true
                            },
                            DataRegionConfigurations = new[]
                {
                        new DataRegionConfiguration
                        {
                            // Persistence is off by default.
                            Name = "inMemoryRegion"
                        }
                }
                        },
                        CacheConfiguration = new[]
            {
                        new CacheConfiguration
                        {
                            // Default data region has persistence enabled.
                            Name = "persistentCache"
                        },
                                new CacheConfiguration
                        {
                            Name = "inMemoryOnlyCache",
                            DataRegionName = "inMemoryRegion"
                        }
            }
                    };

            Console.WriteLine("yuppp...Its Working !!!");
            Console.ReadLine();
          }
    }
}
