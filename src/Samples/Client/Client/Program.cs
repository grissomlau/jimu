using System;
using Autofac;
using Jimu;
using Jimu.Client;
using Simple.IServices;

namespace Simple.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello client!");

            var container = new ContainerBuilder();
            var host = new ServiceHostClientBuilder(container)
                .UseLog4netLogger(new Log4netOptions { EnableConsoleLog = true })
                .UsePollingAddressSelector()
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-")
                .UseDotNettyForTransfer()
                .UseToken(() => "token")
                .UseServiceProxy(new[] { "Simple.IServices" })
                .Build();
            host.Run();
            var proxy = host.Container.Resolve<IServiceProxy>();
            var user = proxy.GetService<IUserService>();
            var name = user.GetName().Result;
            Console.WriteLine("Name is " + name);
            var id = user.GetId();
            Console.WriteLine("id is " + id);
            user.SetId();
            user.SetName("haha");
            Console.ReadKey();


        }
    }
}
