using System;
using Autofac;
using Jimu;
using Jimu.Server;

namespace Server2.HttpForTransfer
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
                .UseHttpForTransfer("127.0.0.1", 8004)
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-", "127.0.0.1:8004")
                ;
            using (var hostJimu = builder.Build())
            {
                hostJimu.Run();
            }

        }
    }
}
