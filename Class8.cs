using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignite
{
    class Car
    {
        [QuerySqlField]
        public string Model { get; set; }
        [QuerySqlField]
        public int Power { get; set; }
    }

    class Class8
    {
        static void Main(string[] args)
        {
            IIgnite ignite = Ignition.Start();

            var queryEntity = new QueryEntity(typeof(int), typeof(Car));
            var cacheConfig = new CacheConfiguration("cars", queryEntity);
            ICache<int, Car> cache = ignite.GetOrCreateCache<int, Car>(cacheConfig);

            
            var insertQuery = new SqlFieldsQuery("INSERT INTO Car (_key, Model, Power) VALUES " +
                                                 "(3, 'Ariel Atom', 350), " +
                                                 "(4, 'Reliant Robin', 39)");
            cache.Query(insertQuery).GetAll();

            var selQuery = new SqlQuery(typeof(Car), "SELECT * FROM Car ORDER BY Power");
            foreach (ICacheEntry<int, Car> entry in cache.Query(selQuery))
                Console.WriteLine(entry);
        }
    }
}
