using Apache.Ignite.Core;
using Apache.Ignite.Core.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignite
{
    class LifecycleExampleHandler : ILifecycleHandler
    {
        public void OnLifecycleEvent(LifecycleEventType evt)
        {
            if (evt == LifecycleEventType.AfterNodeStart)
                Started = true;
            else if (evt == LifecycleEventType.AfterNodeStop)
                Started = false;
        }

        public bool Started { get; private set; }
    }
    class Class3
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                LifecycleHandlers = new[] { new LifecycleExampleHandler() }
            };

            using (var ignite = Ignition.Start(cfg))
            {
                LifecycleExampleHandler l = new LifecycleExampleHandler();
                if (l.Started)
                    Console.WriteLine("After node start");
                else
                    Console.WriteLine("After node stop");
            }
        }
    }
}
