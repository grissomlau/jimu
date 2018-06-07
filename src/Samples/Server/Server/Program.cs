using System;
using Autofac;
using Jimu;
using Jimu.Server;

namespace Simple.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .UseLog4netLogger(new Log4netOptions
                {
                    EnableConsoleLog = true
                })
                .LoadServices(new[] { "Simple.IServices", "Simple.Services" })
                //.UseNetCoreHttpServer("127.0.0.1", 8010)
                .UseDotNettyForTransfer("127.0.0.1", 8010, server => { })
                .UseInMemoryForDiscovery()
                //.UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-", "127.0.0.1:8010")
                ;
            using (var host = builder.Build())
            {
                host.Run();
                Console.WriteLine("Server start successful.");
                Console.ReadLine();
            }
        }
    }
}
