using System;
using Autofac;
using Jimu.Server.Transport.Http;
using Microsoft.AspNetCore.Hosting;

namespace Jimu.Server
{
    public static partial class ApplicationBuilderExtension
    {
        public static IApplicationServerBuilder UseHttpForTransfer(this IApplicationServerBuilder serviceHostBuilder, HttpOptions options, Action<IServer> action = null, Action<IWebHostBuilder> builderAction = null)
        {
            serviceHostBuilder.RegisterComponent(containerBuilder =>
            {
                containerBuilder.RegisterType<HttpServer>().As<IServer>().WithParameter("ip", options.Ip).WithParameter("port", options.Port).WithParameter("builderAction", builderAction).SingleInstance();
            });

            serviceHostBuilder.AddRunner(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use http for transfer");

                var server = container.Resolve<IServer>();
                var routes = server.GetServiceRoutes();
                server.StartAsync();
                action?.Invoke(server);
            });

            return serviceHostBuilder;
        }

    }
}
