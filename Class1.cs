using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using System;

namespace Ignite
{
    class Class1
    {
        static void Main(string[] args)
        {
            IIgnite ignite = Ignition.Start();
            ICache<int, string> c = ignite.GetOrCreateCache<int, string>("test");
            if (c.PutIfAbsent(1, "yash"))
                Console.WriteLine("cache created!");
            else
                Console.WriteLine("cache not created!");

            Console.ReadKey();
        }
    }
}
