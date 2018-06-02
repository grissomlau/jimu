using System;
using Autofac;
using Jimu.Common.Logger.Log4netIntegration;
using Jimu.Server;

namespace Simple.HttpServer
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
                //.UseNetCoreHttpServer("127.0.0.1", 8080)
                .UseDotNettyServer("127.0.0.1", 8080, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "MService-", "127.0.0.1:8080")
                ;
            using (var hostJimu = builder.Build())
            {
                Console.WriteLine("Server start successfulhaha.");

                //  var host = new WebHostBuilder()
                //.UseKestrel()
                //.UseSetting("detailedErrors", "true")
                //.UseContentRoot(Directory.GetCurrentDirectory())
                //.UseUrls("http://localhost:8080")
                ////.UseIISIntegration()
                //.UseStartup<Startup>()
                //.Build();

                //  host.Run();
                hostJimu.Run();

                Console.ReadLine();
            }

        }
    }
}
