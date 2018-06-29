using System;
using Autofac;
using Jimu.Server.Transport.Http;
using Microsoft.AspNetCore.Hosting;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseHttpForTransfer(this IServiceHostServerBuilder serviceHostBuilder, string ip, int port, Action<IServer> action = null, Action<IWebHostBuilder> builderAction = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<HttpServer>().As<IServer>().WithParameter("ip", ip).WithParameter("port", port).WithParameter("builderAction", builderAction).SingleInstance();
            });

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
