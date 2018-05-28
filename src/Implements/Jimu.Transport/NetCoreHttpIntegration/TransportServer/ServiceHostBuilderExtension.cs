using System;
using Autofac;
using Jimu.Common.Transport.NetCoreHttpIntegration.TransportServer;
using Jimu.Core.Server;
using Jimu.Core.Server.TransportServer;
using Microsoft.AspNetCore.Hosting;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseNetCoreHttpServer(this IServiceHostServerBuilder serviceHostBuilder, string ip, int port, Action<IServer> action = null, Action<IWebHostBuilder> builderAction = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<NetCoreHttpServer>().As<IServer>().WithParameter("ip", ip).WithParameter("port", port).WithParameter("builderAction", builderAction).SingleInstance();
            });

            //serviceHostBuilder.AddInitializer(container =>
            //{
            //    var server = container.Resolve<IServer>();
            //    server.StartAsync();
            //    action?.Invoke(server);
            //});

            serviceHostBuilder.AddRunner(container =>
            {
                var server = container.Resolve<IServer>();
                var routes = server.GetServiceRoutes();
                server.StartAsync();
                action?.Invoke(server);
            });

            return serviceHostBuilder;
        }

    }
}
