using Apache.Ignite.Core.Compute;
using Apache.Ignite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignite
{
    class Class4
    {
      
        static void Main(string[] args)
        {
            IIgnite ignite = Ignition.Start();
            ICompute compute = ignite.GetCompute();

            // Execute a job and wait for the result.
            int res = compute.Call(new ComputeFunc());

            Console.WriteLine(res);
        }
    }

    internal class ComputeFunc : IComputeFunc<int>
    {
        public int Invoke()
        {
            int k = 0;
            for (int i = 0; i < 100; i++)
            {
                k += i;
            }
            return k;
        }
    }
}
