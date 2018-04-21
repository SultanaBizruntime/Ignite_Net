using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Affinity;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using Apache.Ignite.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitedotnet
{
    class LINQ
    {
        /// <summary>Organization cache name.</summary>
        private const string OrganizationCacheName = "cache query-organization";

        /// <summary>Employee cache name.</summary>
        private const string EmployeeCacheName = "cache query-employee";

        /// <summary>Colocated employee cache name.</summary>
        private const string EmployeeCacheNameColocated = "cache query-employee-colocated";

        [STAThread]
        public static void Main()
        {
            using (var ignite = Ignition.Start())
            {
                Console.WriteLine();
                Console.WriteLine(">>> Cache LINQ example started.");

                var employeeCache = ignite.GetOrCreateCache<int, Employee>(
                    new CacheConfiguration(EmployeeCacheName, typeof(Employee)));

                var employeeCacheColocated = ignite.GetOrCreateCache<AffinityKey, Employee>(
                    new CacheConfiguration(EmployeeCacheNameColocated, typeof(Employee)));

                var organizationCache = ignite.GetOrCreateCache<int, Organization>(
                    new CacheConfiguration(OrganizationCacheName, new QueryEntity(typeof(int), typeof(Organization))));

                    // Populate cache with sample data entries.
                PopulateCache(employeeCache);
                PopulateCache(employeeCacheColocated);
                PopulateCache(organizationCache);

                // Run SQL query example.
                QueryExample(employeeCache);

                // Run compiled SQL query example.
                CompiledQueryExample(employeeCache);

                // Run SQL query with join example.
                JoinQueryExample(employeeCacheColocated, organizationCache);

                // Run SQL query with distributed join example.
                DistributedJoinQueryExample(employeeCache, organizationCache);

                // Run SQL fields query example.
                FieldsQueryExample(employeeCache);

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine(">> press any key to exit ...");
            Console.ReadKey();
        }

        /// <summary>
        /// Queries employees that have provided ZIP code in address.
        /// </summary>
        private static void QueryExample(ICache<int, Employee> cache)
        {
            const int zip = 23564;

            IQueryable<ICacheEntry<int, Employee>> qry =
                cache.AsCacheQueryable().Where(emp => emp.Value.Address.Zip == zip);

            Console.WriteLine(">>> Employees with zipcode " + zip + ":");

            foreach (ICacheEntry<int, Employee> entry in qry)
                Console.WriteLine(">>>    " + entry.Value);
        }

        /// <summary>
        /// Queries employees that have provided ZIP code in address with a compiled query.
        /// </summary>
        private static void CompiledQueryExample(ICache<int, Employee> cache)
        {
            const int zip = 23564;

            var cache0 = cache.AsCacheQueryable();

            // Compile cache query to eliminate LINQ overhead on multiple runs.
            Func<int, IQueryCursor<ICacheEntry<int, Employee>>> qry =
                CompiledQuery.Compile((int z) => cache0.Where(emp => emp.Value.Address.Zip == z));

            Console.WriteLine(">>> Employees with zipcode {0} using compiled query:", zip);

            foreach (ICacheEntry<int, Employee> entry in qry(zip))
                Console.WriteLine(">>>    " + entry.Value);
        }

        /// <summary>
        /// Queries employees that work for organization with provided name.
        /// </summary>
        private static void JoinQueryExample(ICache<AffinityKey, Employee> employeeCache,
            ICache<int, Organization> organizationCache)
        {
            const string orgName = "Apache";

            IQueryable<ICacheEntry<AffinityKey, Employee>> employees = employeeCache.AsCacheQueryable();
            IQueryable<ICacheEntry<int, Organization>> organizations = organizationCache.AsCacheQueryable();

            IQueryable<ICacheEntry<AffinityKey, Employee>> qry =
                from employee in employees
                from organization in organizations
                where employee.Value.OrganizationId == organization.Key && organization.Value.Name == orgName
                select employee;

            Console.WriteLine(">>> Employees working for " + orgName + ":");

            foreach (ICacheEntry<AffinityKey, Employee> entry in qry)
                Console.WriteLine(">>>     " + entry.Value);
        }

        /// <summary>
        /// Queries employees that work for organization with provided name.
        /// </summary>
        private static void DistributedJoinQueryExample(ICache<int, Employee> employeeCache,
            ICache<int, Organization> organizationCache)
        {
            const string orgName = "Apache";

            var queryOptions = new QueryOptions { EnableDistributedJoins = true };

            IQueryable<ICacheEntry<int, Employee>> employees = employeeCache.AsCacheQueryable(queryOptions);
            IQueryable<ICacheEntry<int, Organization>> organizations = organizationCache.AsCacheQueryable(queryOptions);

            IQueryable<ICacheEntry<int, Employee>> qry =
                from employee in employees
                from organization in organizations
                where employee.Value.OrganizationId == organization.Key && organization.Value.Name == orgName
                select employee;

            Console.WriteLine(">>> Employees working for " + orgName + ":");

            foreach (ICacheEntry<int, Employee> entry in qry)
                Console.WriteLine(">>>     " + entry.Value);
        }

        /// <summary>
        /// Queries names and salaries for all employees.
        /// </summary>
        private static void FieldsQueryExample(ICache<int, Employee> cache)
        {
            var qry = cache.AsCacheQueryable().Select(entry => new { entry.Value.Name, entry.Value.Salary });

            Console.WriteLine();
            Console.WriteLine(">>> Employee names and their salaries:");

            foreach (var row in qry)
                Console.WriteLine(">>>     [Name=" + row.Name + ", salary=" + row.Salary + ']');
        }

        /// <summary>
        /// Populate cache with data for this example.
        /// </summary>
        private static void PopulateCache(ICache<int, Organization> cache)
        {
            cache.Put(1, new Organization(
                "Apache",
                new Address("1065 East Hillsdale Blvd, Foster City, CA", 94404),
                OrganizationType.Private,
                DateTime.Now));

            cache.Put(2, new Organization(
                "Microsoft",
                new Address("1096 Eddy Street, San Francisco, CA", 23564),
                OrganizationType.Private,
                DateTime.Now));
        }

        /// <summary>
        /// Populate cache with data for this example.
        /// </summary>
        private static void PopulateCache(ICache<AffinityKey, Employee> cache)
        {
            cache.Put(new AffinityKey(1, 1), new Employee(
                "asdf",
                12500,
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                new[] { "Human Resources", "Customer Service" },
                1));

            cache.Put(new AffinityKey(2, 1), new Employee(
                "qwer",
                11000,
                new Address("184 Fidler Drive, San Antonio, TX", 78130),
                new[] { "Development", "QA" },
                1));

            cache.Put(new AffinityKey(3, 1), new Employee(
                "nbeh",
                12500,
                new Address("667 Jerry Dove Drive, Florence, SC", 29501),
                new[] { "Logistics" },
                1));

            cache.Put(new AffinityKey(4, 2), new Employee(
                "polmgd",
                25300,
                new Address("2702 Freedom Lane, San Francisco, CA", 94109),
                new[] { "Development" },
                2));
        }

        /// <summary>
        /// Populate cache with data for this example.
        /// </summary>
        /// <param name="cache">Cache.</param>
        private static void PopulateCache(ICache<int, Employee> cache)
        {
            cache.Put(1, new Employee(
                "asdf",
                12500,
                new Address("1096 Eddy Street, San Francisco, CA", 94109),
                new[] { "Human Resources", "Customer Service" },
                1));

            cache.Put(2, new Employee(
                "qwer",
                11000,
                new Address("184 Fidler Drive, San Antonio, TX", 78130),
                new[] { "Development", "QA" },
                1));

            cache.Put(3, new Employee(
                "nbeh",
                12500,
                new Address("667 Jerry Dove Drive, Florence, SC", 29501),
                new[] { "Logistics" },
                1));

            cache.Put(4, new Employee(
                "polmgd",
                25300,
                new Address("2702 Freedom Lane, San Francisco, CA", 94109),
                new[] { "Development" },
                2));
       }
    }
}
