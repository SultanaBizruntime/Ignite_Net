﻿using Apache.Ignite.Core;
using Apache.Ignite.Core.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apache.Ignite.Core.Binary;

namespace Ignitedotnet
{
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override string ToString()
        {
            return $"Person [Name={Name}, Age={Age}]";
        }
    }
    class BinaryConfigurationDemo
    {
        static void Main()
        {
            Sample();
            Console.ReadKey();
        }
        static void Sample()
        {
            var cfg = new IgniteConfiguration
            {
                // Register custom class for Ignite serialization
                BinaryConfiguration = new BinaryConfiguration(typeof(Person))
            };
            IIgnite ignite = Ignition.Start(cfg);

            ICache<int, Person> cache = ignite.GetOrCreateCache<int, Person>("persons");
            cache[1] = new Person { Name = "Micky Mouse", Age = 25 };
            cache[2] = new Person { Name = "lmn",Age=27};
            foreach (ICacheEntry<int, Person> cacheEntry in cache)
                Console.WriteLine(cacheEntry);
        }
    }
}
