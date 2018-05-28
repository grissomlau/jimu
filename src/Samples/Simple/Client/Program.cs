using System;
using Autofac;
using Jimu;
using Jimu.Common.Logger.Log4netIntegration;
using Jimu.Core.Client;
using Jimu.Core.Client.LoadBalance;
using Jimu.Core.Client.Proxy;
using Jimu.Core.Client.RemoteInvoker;
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
                .UseConsul("127.0.0.1", 8500, "MService-")
                .UseDotNettyClient()
                .UseRemoteServiceInvoker(() => "token")
                .UseServiceProxy(new[] { "Simple.IServices" })
                .Build();
            builder.Run();

            var user = ServiceProxy.GetService<IUserService>();
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
