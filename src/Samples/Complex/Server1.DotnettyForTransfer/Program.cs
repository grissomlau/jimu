using System;
using Autofac;
using Jimu;
using Jimu.Common.Logger;
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
                .UseDotNettyForTransfer("127.0.0.1", 8005, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService", "127.0.0.1:8005")
                ;
            using (var hostJimu = builder.Build())
            {
                hostJimu.Run();
                Console.ReadLine();
            }

        }
    }
}
