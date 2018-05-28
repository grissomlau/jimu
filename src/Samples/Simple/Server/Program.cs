using System;
using Autofac;
using Jimu;
using Jimu.Common.Logger.Log4netIntegration;
using Jimu.Core.Server;
using Jimu.Core.Server.ServiceContainer;

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
                .UseDotNettyServer("127.0.0.1", 8009, server => { })
                .UseConsul("127.0.0.1", 8500, "MService-", "127.0.0.1:8009")
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
