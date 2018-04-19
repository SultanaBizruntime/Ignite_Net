using Apache.Ignite.Core;
using Apache.Ignite.Core.Discovery.Tcp;
using Apache.Ignite.Core.Discovery.Tcp.Multicast;
using System;


namespace Ignite
{
    class Class6
    {
        static void Main(string[] args)
        {
            var cfg = new IgniteConfiguration
            {
                DiscoverySpi = new TcpDiscoverySpi
                {
                    IpFinder = new TcpDiscoveryMulticastIpFinder
                    {
                        MulticastGroup = "192.168.137.1"
                    }
                }
            };
            Console.ReadKey();
        }
    }
}
