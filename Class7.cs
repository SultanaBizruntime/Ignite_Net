using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Cache.Query;
using System;
using System.Data.SqlClient;

namespace Ignite
{
    class Class7
    {
        public class Person
        {
            /** Indexed field. Will be visible for SQL engine. */
            [QuerySqlField(IsIndexed = true)]
            public long _id;

            /** Queryable field. Will be visible for SQL engine. */
            [QuerySqlField]
            public string _name;

            public int _age;
        }

        static void Main(string[] args)
        {
            //var con = new SqlConnection(@"Data Source=.\YASWANTH;Persist Security Info=True;User ID=sa;Password=yash1234");
            //con.Open();
            QueryEntity q = new QueryEntity();
            IIgnite ignite = Ignition.Start();

            var cache = ignite.GetOrCreateCache<int, Person>("personCache");

            // Execute query to get names of all employees.
            var sql = new SqlFieldsQuery(
                "select concat(Name) from employee as p");

            // Iterate over the result set.
            foreach (var fields in cache.Query(sql))
                Console.WriteLine("Person Name = {0}", fields[0]);
        }
    }
}
