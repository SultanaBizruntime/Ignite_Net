using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using Apache.Ignite.Core.Cache.Query;
using System;

namespace Ignite
{
    class Class9
    {
        static void Main(string[] args)
        {
            IIgnite ignite = Ignition.Start();
            //IgniteCache<Long, Person> cache = ignite.cache("mycache");

            //SqlFieldsQuery sql = new SqlFieldsQuery(
            //  "select concat(firstName, ' ', lastName) from Person");

            //// Select concatinated first and last name for all persons.
            //try (QueryCursor < List <?>> cursor = cache.query(sql)) {
            //    for (List <?> row : cursor)
            //        System.out.println("Full name: " + row.get(0));
            //}

            //ICache<long, Person> personCache = ignite.cache("personCache");

            // Select with join between Person and Organization to
            // get the names of all the employees of a specific organization.
            //SqlFieldsQuery sql = new SqlFieldsQuery(
            //    "select p.name  "
            //        + "from Person p, \"orgCache\".Organization o where "
            //        + "p.orgId = o.id "
            //        + "and o.name = ?");

            //// Execute the query and obtain the query result cursor.
            //try (QueryCursor < List <?>> cursor = personCache.query(sql.setArgs("Ignite"))) {
            //    for (List <?> row : cursor)
            //        System.out.println("Person name=" + row);
            //}
        }
    }
}
