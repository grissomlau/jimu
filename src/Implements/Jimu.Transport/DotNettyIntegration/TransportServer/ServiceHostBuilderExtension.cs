using System;
using Autofac;
using Jimu.Common.Transport.DotNettyIntegration;
using Jimu.Common.Transport.DotNettyIntegration.TransportServer;
using Jimu.Core.Server;
using Jimu.Core.Server.TransportServer;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseDotNettyServer(this IServiceHostServerBuilder serviceHostBuilder, string ip, int port, Action<IServer> action = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<DotNettyServer>().As<IServer>().WithParameter("address", new DotNettyAddress(ip, port)).SingleInstance();
            });

            serviceHostBuilder.AddInitializer(container =>
            {
                var server = container.Resolve<IServer>();
                server.StartAsync();
                action?.Invoke(server);
            });

            return serviceHostBuilder;
        }

    }
}
