using System;
using Autofac;
using Jimu.Server.Transport.DotNetty;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseDotNettyForTransfer(this IServiceHostServerBuilder serviceHostBuilder, string ip, int port, Action<IServer> action = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<DotNettyServer>().As<IServer>().WithParameter("address", new DotNettyAddress(ip, port)).SingleInstance();
            });

            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use dotnetty for transfer");
                var server = container.Resolve<IServer>();
                server.StartAsync();
                action?.Invoke(server);
            });

            return serviceHostBuilder;
        }

    }
}
