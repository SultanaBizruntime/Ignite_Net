using Apache.Ignite.Core;
using Apache.Ignite.Core.Cluster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignite
{
    class Class5
    {
        static void Main(string[] args)
        {
            IIgnite ignite = Ignition.Start();

            ICluster cluster = ignite.GetCluster();
        }
    }
}
