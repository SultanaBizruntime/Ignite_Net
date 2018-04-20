using Apache.Ignite.Core;
using Apache.Ignite.Core.Compute;
using Apache.Ignite.Core.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitedotnet
{
    class AssemblyMode
    {
        static void Main()
        {
            Test();
            Console.ReadKey();
        }
        static void Test()
        { 
            var cfg = new IgniteConfiguration
            {
                PeerAssemblyLoadingMode = PeerAssemblyLoadingMode.CurrentAppDomain,
            };
            using (var ignite = Ignition.Start(cfg))
            {
                ignite.GetCompute().Broadcast(new MyAction());
            }
        }
        class MyAction : IComputeAction
        {
            public void Invoke() => Console.WriteLine("Ignite is a multi language support");
        }
    }
}
