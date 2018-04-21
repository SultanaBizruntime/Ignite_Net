using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitedotnet
{
    class Sqlddl
    {
        private const string CacheName = "SqlDDL Example";
        static void Main()
        {
            using (var ignite = Ignition.Start())
            {
                Console.WriteLine(">> cache query SQL DDL example");
                //create a cache to act as an entry point SQL queries
                var cachecfg = new CacheConfiguration(CacheName)
                {
                    SqlSchema="PUBLIC",
                    CacheMode=CacheMode.Replicated
                };
                ICache<object, object> cache = ignite.GetOrCreateCache<object, object>(cachecfg);
                // Create reference City table based on REPLICATED template.
                cache.Query(new SqlFieldsQuery(
                    "CREATE TABLE city (id LONG PRIMARY KEY, name VARCHAR) WITH \"template=replicated\"")).GetAll();

                // Create table based on PARTITIONED template with one backup.
                cache.Query(new SqlFieldsQuery(
                    "CREATE TABLE person (id LONG, name VARCHAR, city_id LONG, PRIMARY KEY (id, city_id)) " +
                    "WITH \"backups=1, affinity_key=city_id\"")).GetAll();

                // Create an index.
                cache.Query(new SqlFieldsQuery("CREATE INDEX on Person (city_id)")).GetAll();

                Console.WriteLine("\n>>> Created database objects.");

                const string addCity = "INSERT INTO city (id, name) VALUES (?, ?)";

                cache.Query(new SqlFieldsQuery(addCity, 1, "qewe"));
                cache.Query(new SqlFieldsQuery(addCity, 2, "zxcf"));
                cache.Query(new SqlFieldsQuery(addCity, 3, "lkj"));

                const string addPerson = "INSERT INTO person (id, name, city_id) values (?, ?, ?)";

                cache.Query(new SqlFieldsQuery(addPerson, 1, "John ", 3));
                cache.Query(new SqlFieldsQuery(addPerson, 2, "Micky", 2));
                cache.Query(new SqlFieldsQuery(addPerson, 3, "sdnj", 1));
                cache.Query(new SqlFieldsQuery(addPerson, 4, "Richard", 2));

                Console.WriteLine("\n>>> Populated data.");

                IFieldsQueryCursor res = cache.Query(new SqlFieldsQuery(
                    "SELECT p.name, c.name FROM Person p INNER JOIN City c on c.id = p.city_id"));

                Console.WriteLine("\n>>> Query results:");

                foreach (var row in res)
                {
                    Console.WriteLine("{0}, {1}", row[0], row[1]);
                }

                cache.Query(new SqlFieldsQuery("drop table Person")).GetAll();
                cache.Query(new SqlFieldsQuery("drop table City")).GetAll();

                Console.WriteLine("\n>>> Dropped database objects.");
            }

            Console.WriteLine();
            Console.WriteLine(">>> Example finished, press any key to exit ...");
            Console.ReadKey();
        }
    }
}
