using System;
using Autofac;
using Jimu.Client;
using Jimu.Common.Logger.Log4netIntegration;
using Simple.IServices;

namespace Simple.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello client!");

            var container = new ContainerBuilder();
            var builder = new ServiceHostClientBuilder(container)
                .UseLog4netLogger(new Log4netOptions { EnableConsoleLog = true })
                .UsePollingAddressSelector()
                .UseConsulForDiscovery("127.0.0.1", 8500, "MService-")
                .UseDotNettyClient()
                .UseToken(() => "token")
                .UseServiceProxy(new[] { "Simple.IServices" })
                .Build();
            builder.Run();
            var proxy = builder.Container.Resolve<IServiceProxy>();
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
