using System;
using Autofac;
using Jimu;
using Jimu.Server;

namespace Server1.DotnettyForTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .UseLog4netLogger(new LogOptions
                {
                    EnableConsoleLog = true
                })
                .LoadServices(new[] { "IServices", "Services" })
                .UseDotNettyForTransfer(new Jimu.Server.Transport.DotNetty.DotNettyOptions("127.0.0.1", 8003), server => { })
                .UseConsulForDiscovery(new Jimu.Server.Discovery.ConsulIntegration.ConsulOptions("127.0.0.1", 8500, "JimuService-", "127.0.0.1:8003"))
                ;
            using (var hostJimu = builder.Build())
            {
                hostJimu.Run();
                Console.ReadLine();
            }

        }
    }
}
