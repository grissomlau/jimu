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
                .UseInServerForDiscovery(new HttpAddress("127.0.0.1", 8007))
                .UseHttpForTransfer()
                .UseToken(() => "token")
                .UseServiceProxy(new[] { "IServices" })
                .Build();
            host.Run();
            var proxy = host.Container.Resolve<IServiceProxy>();
            var echo = proxy.GetService<IEchoService>();
            var name = echo.GetEcho("test");
            Console.WriteLine("return:  " + name);
            Console.ReadKey();


        }
    }
}
