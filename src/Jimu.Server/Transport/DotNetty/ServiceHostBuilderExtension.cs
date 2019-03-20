using System;
using Autofac;
using Jimu.Server.Transport.DotNetty;

namespace Jimu.Server
{
    public static partial class ApplicationBuilderExtension
    {
        public static IApplicationServerBuilder UseDotNettyForTransfer(this IApplicationServerBuilder serviceHostBuilder, DotNettyOptions options, Action<IServer> action = null)
        {
            serviceHostBuilder.RegisterComponent(containerBuilder =>
            {
                containerBuilder.RegisterType<DotNettyServer>().As<IServer>().WithParameter("address", new DotNettyAddress(options.Ip, options.Port)).SingleInstance();
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
