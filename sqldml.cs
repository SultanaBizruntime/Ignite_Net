using System;
using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;

namespace Ignitedotnet
{
    class sqldml
    {
        /// <summary>Organization cache name.</summary>
        private const string OrganizationCacheName = "cache query-dml-organization";

        /// <summary>Employee cache name.</summary>
        private const string EmployeeCacheName = "cache query-dml-employee";

        [STAThread]
        public static void Main()
        {
            using (var ignite = Ignition.Start())
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache query DML example started.");

                var employeeCache = ignite.GetOrCreateCache<int, Employee>(
                    new CacheConfiguration(EmployeeCacheName, new QueryEntity(typeof(int), typeof(Employee))));

                var organizationCache = ignite.GetOrCreateCache<int, Organization>(new CacheConfiguration(
                    OrganizationCacheName, new QueryEntity(typeof(int), typeof(Organization))));

                employeeCache.Clear();
                organizationCache.Clear();

                Insert(organizationCache, employeeCache);
                Select(employeeCache, "Inserted data");

                Update(employeeCache);
                Select(employeeCache, "Update salary for ASF employees");

                Delete(employeeCache);
                Select(employeeCache, "Delete non-ASF employees");

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(">> press any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// Selects and displays Employee data.
        /// </summary>
        /// <param name="employeeCache">Employee cache.</param>
        /// <param name="message">Message.</param>
        private static void Select(ICache<int, Employee> employeeCache, string message)
        {
            Console.WriteLine("\n>>> {0}", message);

            var qry = new SqlFieldsQuery(string.Format(
                "select emp._key, emp.name, org.name, emp.salary " +
                "from Employee as emp, " +
                "\"{0}\".Organization as org " +
                "where emp.organizationId = org.key", OrganizationCacheName))
            {
                EnableDistributedJoins = true
            };

            using (var cursor = employeeCache.Query(qry))
            {
                foreach (var row in cursor)
                {
                    Console.WriteLine(">>> {0}: {1}, {2}, {3}", row[0], row[1], row[2], row[3]);
                }
            }
        }

        /// <summary>
        /// Populate cache with test data.
        /// </summary>
        /// <param name="organizationCache">Organization cache.</param>
        /// <param name="employeeCache">Employee cache.</param>
        private static void Insert(ICache<int, Organization> organizationCache, ICache<int, Employee> employeeCache)
        {
            // Insert organizations.
            var qry = new SqlFieldsQuery("insert into Organization (key, name) values (?, ?)", 1, "ASF");
            organizationCache.Query(qry);

            qry.Arguments = new object[] { 2, "Eclipse" };
            organizationCache.Query(qry);

            // Insert employees.
            qry = new SqlFieldsQuery("insert into Employee (key, name, organizationId, salary) values (?, ?, ?, ?)");

            qry.Arguments = new object[] { 1, "mnb", 1, 4000 };
            employeeCache.Query(qry);

            qry.Arguments = new object[] { 2, "qwe", 1, 5000 };
            employeeCache.Query(qry);

            qry.Arguments = new object[] { 3, "xzdfg", 2, 2000 };
            employeeCache.Query(qry);

            qry.Arguments = new object[] { 4, "puort", 2, 3000 };
            employeeCache.Query(qry);
        }

        /// <summary>
        /// Conditional UPDATE query: raise salary for ASF employees.
        /// </summary>
        /// <param name="employeeCache">Employee cache.</param>
        private static void Update(ICache<int, Employee> employeeCache)
        {
            var qry = new SqlFieldsQuery("update Employee set salary = salary * 1.1 where organizationId = ?", 1);
            employeeCache.Query(qry);
        }

        /// <summary>
        /// Conditional DELETE query: remove non-ASF employees.
        /// </summary>
        /// <param name="employeeCache">Employee cache.</param>
        private static void Delete(ICache<int, Employee> employeeCache)
        {
            var qry = new SqlFieldsQuery("delete from Employee where organizationId != ?", 1);
            employeeCache.Query(qry);
        }
    }
}
