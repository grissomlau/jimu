using Autofac;
using Jimu.Common.Discovery.ConsulIntegration;
using Jimu.Server;
using System;
using System.Threading;

namespace Jimu.Server
{
    public static class ServiceHostBuilderExtension
    {
        /// <summary>
        /// use consul for discovery server
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <param name="consulIp">server ip</param>
        /// <param name="consulPort">server port</param>
        /// <param name="serviceCategory">server category name</param>
        /// <param name="serverAddress">server address</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder UseConsulForDiscovery(this IServiceHostServerBuilder serviceHostBuilder, string consulIp, int consulPort, string serviceCategory, string serverAddress = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>().WithParameter("ip", consulIp).WithParameter("port", consulPort).WithParameter("serviceCategory", serviceCategory).SingleInstance();
            });

            //serviceHostBuilder.AddRunner(async container =>
            serviceHostBuilder.AddInitializer(async container =>
            {
                while (!container.IsRegistered<IServer>())
                {
                    default(SpinWait).SpinOnce();
                }

                IServer server = container.Resolve<IServer>();
                var routes = server.GetServiceRoutes();
                try
                {
                    var discovery = container.Resolve<IServiceDiscovery>();
                    if (!string.IsNullOrEmpty(serverAddress))
                    {
                        await discovery.ClearAsync(serverAddress);
                    }

                    await discovery.SetRoutesAsync(routes);
                }
                catch (Exception ex)
                {
                    var logger = container.Resolve<ILogger>();
                    logger.Error($"error occurred while connecting with consul, ensure consul is running.\r\n", ex);
                }

            });
            return serviceHostBuilder;
        }



    }
}

namespace Jimu.Client
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseConsulForDiscovery(this IServiceHostClientBuilder serviceHostBuilder, string ip, int port, string serviceCategory)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>().WithParameter("ip", ip).WithParameter("port", port).WithParameter("serviceCategory", serviceCategory).SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var clientDiscovery = container.Resolve<IClientServiceDiscovery>();
                var serverDiscovery = container.Resolve<IServiceDiscovery>();
                clientDiscovery.AddRoutesGetter(serverDiscovery.GetRoutesAsync);
            });

            return serviceHostBuilder;
        }
    }
}
