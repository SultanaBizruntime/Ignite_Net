using System;
using System.Linq;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Apache.Ignite.Linq;

namespace Ignite
{

    class Class11
    {
        private const string CacheName = "thinClientSqlCache";

        [STAThread]
        public static void Main()
        {
            var cfg = new IgniteClientConfiguration
            {
                Host = "192.168.137.1"
            };

            using (IIgniteClient igniteClient = Ignition.StartClient(cfg))
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache query client example started.");

                // Configure query entities to enable SQL.
                var cacheCfg = new CacheClientConfiguration
                {
                    Name = CacheName,
                    QueryEntities = new[]
                    {
                        new QueryEntity(typeof(int), typeof(Employee)),
                    }
                };

                ICacheClient<int, Employee> cache = igniteClient.GetOrCreateCache<int, Employee>(cacheCfg);

                // Populate cache with sample data entries.
                PopulateCache(cache);

                // Run SQL example.
                SqlQueryExample(cache);
                LinqExample(cache);

                // Run SQL fields query example.
                SqlFieldsQueryExample(cache);
                LinqFieldsExample(cache);
            }

            Console.WriteLine();
            Console.WriteLine(">>> Example finished, press any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// Queries employees that have provided ZIP code in address.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void SqlQueryExample(ICacheClient<int, Employee> cache)
        {
            const int zip = 94109;

            var qry = cache.Query(new SqlQuery(typeof(Employee), "zip = ?", zip));

            Console.WriteLine();
            Console.WriteLine(">>> Employees with zipcode {0} (SQL):", zip);

            foreach (var entry in qry)
                Console.WriteLine(">>>    " + entry.Value);
        }

        /// <summary>
        /// Queries employees that have provided ZIP code in address.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void LinqExample(ICacheClient<int, Employee> cache)
        {
            const int zip = 94109;

            IQueryable<ICacheEntry<int, Employee>> qry =
                 //cache.AsCacheQueryable().Where(emp => emp.Value.Address.Zip == zip);
                 cache.AsCacheQueryable();

            Console.WriteLine();
            Console.WriteLine(">>> Employees with zipcode " + zip + " (LINQ):");

            foreach (ICacheEntry<int, Employee> entry in qry)
                Console.WriteLine(">>>    " + entry.Value);

            Console.WriteLine();
            Console.WriteLine(">>> Generated SQL:");
            Console.WriteLine(">>> " + qry.ToCacheQueryable().GetFieldsQuery().Sql);
        }


        /// <summary>
        /// Queries names and salaries for all employees.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void SqlFieldsQueryExample(ICacheClient<int, Employee> cache)
        {
            var qry = cache.Query(new SqlFieldsQuery("select name, salary from Employee"));

            Console.WriteLine();
            Console.WriteLine(">>> Employee names and their salaries (SQL):");

            foreach (var row in qry)
                Console.WriteLine(">>>     [Name=" + row[0] + ", salary=" + row[1] + ']');
        }

        /// <summary>
        /// Queries names and salaries for all employees.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void LinqFieldsExample(ICacheClient<int, Employee> cache)
        {
            var qry = cache.AsCacheQueryable().Select(entry => new { entry.Value.Name, entry.Value.Salary });

            Console.WriteLine();
            Console.WriteLine(">>> Employee names and their salaries (LINQ):");

            foreach (var row in qry)
                Console.WriteLine(">>>     [Name=" + row.Name + ", salary=" + row.Salary + ']');

            Console.WriteLine();
            Console.WriteLine(">>> Generated SQL:");
            Console.WriteLine(">>> " + qry.ToCacheQueryable().GetFieldsQuery().Sql);
        }

        /// <summary>
        /// Populate cache with data for this example.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void PopulateCache(ICacheClient<int, Employee> cache)
        {
            cache.Put(1, new Employee(
                "James Wilson",
                12500,
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                new[] { "Human Resources", "Customer Service" },
                1));

            cache.Put(2, new Employee(
                "Daniel Adams",
                11000,
                new Address("184 Fidler Drive, San Antonio, TX", 78130),
                new[] { "Development", "QA" },
                1));

            cache.Put(3, new Employee(
                "Cristian Moss",
                12500,
                new Address("667 Jerry Dove Drive, Florence, SC", 29501),
                new[] { "Logistics" },
                1));

            cache.Put(4, new Employee(
                "Allison Mathis",
                25300,
                new Address("2702 Freedom Lane, San Francisco, CA", 94109),
                new[] { "Development" },
                2));

            cache.Put(5, new Employee(
                "Breana Robbin",
                6500,
                new Address("3960 Sundown Lane, Austin, TX", 78130),
                new[] { "Sales" },
                2));

            cache.Put(6, new Employee(
                "Philip Horsley",
                19800,
                new Address("2803 Elsie Drive, Sioux Falls, SD", 57104),
                new[] { "Sales" },
                2));

            cache.Put(7, new Employee(
                "Brian Peters",
                10600,
                new Address("1407 Pearlman Avenue, Boston, MA", 12110),
                new[] { "Development", "QA" },
                2));
        }
    }

    internal class Address
    {
        private string v1;
        public int v2;

        public Address(string v1, int v2)
        {
            this.v1 = v1;
            this.v2 = v2;
        }
    }

    internal class Employee
    {
        public string Name;
        public int Salary;
        private Address address;
        private string[] v3;
        private int v4;

        public Employee(string v1, int v2, Address address, string[] v3, int v4)
        {
            this.Name = v1;
            this.Salary = v2;
            this.address = address;
            this.v3 = v3;
            this.v4 = v4;
        }
    }
}