using Autofac;
using Jimu.Common.Discovery.ConsulIntegration;
using Jimu.Core.Client;
using Jimu.Core.Commons.Discovery;
using Jimu.Core.Server;
using Jimu.Core.Server.TransportServer;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseConsul(this IServiceHostServerBuilder serviceHostBuilder, string ip, int port, string serviceCategory, string serverAddress = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>().WithParameter("ip", ip).WithParameter("port", port).WithParameter("serviceCategory", serviceCategory).SingleInstance();
            });

            serviceHostBuilder.AddInitializer(async container =>
            {
                if (container.IsRegistered<IServer>())
                {
                    IServer server = container.Resolve<IServer>();
                    var routes = server.GetServiceRoutes();
                    //IServiceDiscovery serviceDiscovery = container.Resolve<IServiceDiscovery>();
                    //serviceDiscovery.SetRoutesAsync(routes);

                    var discovery = container.Resolve<IServiceDiscovery>();
                    if (!string.IsNullOrEmpty(serverAddress))
                    {
                        await discovery.ClearAsync(serverAddress);
                    }
                    await discovery.SetRoutesAsync(routes);
                }

            });
            return serviceHostBuilder;
        }



        public static IServiceHostClientBuilder UseConsul(this IServiceHostClientBuilder serviceHostBuilder, string ip, int port, string serviceCategory)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>().WithParameter("ip", ip).WithParameter("port", port).WithParameter("serviceCategory", serviceCategory).SingleInstance();
            });
            return serviceHostBuilder;
        }
    }
}
