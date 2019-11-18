using Autofac;
using Jimu;
using Jimu.Client;
using Jimu.Client.Proxy;
using Jimu.Server;
using Microsoft.Extensions.Hosting;
using System;

namespace Order.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Order Server start...");
            //IHostBuilder thisHostBuilder = null;
            //ApplicationHostServer.Instance.BuildHost((hostBuilder, container) =>
            //{
            //    thisHostBuilder = hostBuilder;
            //}).BuildJimu(builder =>
            //{
            //    IApplication hostClient = null;
            //    builder.AddServiceModule(x =>
            //    {
            //        x.Register(c => hostClient.Container.Resolve<IServiceProxy>()).As<IServiceProxy>();
            //    }).AddRunner(x =>
            //    {
            //        hostClient = new ApplicationClientBuilder(new ContainerBuilder()).Build();
            //        hostClient.RunInServer(thisHostBuilder);
            //    });
            //}).Run();

            ApplicationServerClient.Instance.Run();

        }
    }
}
