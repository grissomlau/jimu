using System;
using Autofac;
using Jimu;
using Jimu.Client;
using IServices;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello client!");

            var container = new ContainerBuilder();
            var host = new ServiceHostClientBuilder(container)
                .UseLog4netLogger(new LogOptions { EnableConsoleLog = true })
                .UsePollingAddressSelector()
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-")
                .UseHttpForTransfer()
                //.UseDotNettyForTransfer()
                //.UseToken(() => "token")
                .UseServiceProxy(new[] { "IServices" })
                .Build();
            host.Run();
            var proxy = host.Container.Resolve<IServiceProxy>();
            while (Console.Read() != 'q')
            {
                var echo = proxy.GetService<IEchoService>();
                var name = echo.GetEcho("test");
                Console.WriteLine("return:  " + name);
            }

            Console.ReadKey();


        }
    }
}
