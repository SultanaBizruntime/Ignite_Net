using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;

namespace Ignite
{
    public class SqlDdlExample
    {
        /// <summary>Dummy cache name.</summary>
        private const string DummyCacheName = "dummy_cache";

        [STAThread]
        static void Main(string[] args)
        {
            using (var ignite = Ignition.Start())
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache query DDL example started.");

                // Create dummy cache to act as an entry point for SQL queries (new SQL API which do not require this
                // will appear in future versions, JDBC and ODBC drivers do not require it already).
                var cacheCfg = new CacheConfiguration(DummyCacheName)
                {
                    SqlSchema = "PUBLIC",
                    CacheMode = CacheMode.Replicated
                };

                ICache<object, object> cache = ignite.GetOrCreateCache<object, object>(cacheCfg);

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

                cache.Query(new SqlFieldsQuery(addCity, 1L, "Forest Hill"));
                cache.Query(new SqlFieldsQuery(addCity, 2L, "Denver"));
                cache.Query(new SqlFieldsQuery(addCity, 3L, "St. Petersburg"));

                const string addPerson = "INSERT INTO person (id, name, city_id) values (?, ?, ?)";

                cache.Query(new SqlFieldsQuery(addPerson, 1L, "John Doe", 3L));
                cache.Query(new SqlFieldsQuery(addPerson, 2L, "Jane Roe", 2L));
                cache.Query(new SqlFieldsQuery(addPerson, 3L, "Mary Major", 1L));
                cache.Query(new SqlFieldsQuery(addPerson, 4L, "Richard Miles", 2L));

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
