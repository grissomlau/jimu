using Autofac;
using Jimu.Client.Discovery.Implement;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder SetDiscoveryAutoUpdateJobInterval(
            this IServiceHostClientBuilder serviceHostBuilder, DiscoveryOptions options)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().WithParameter("updateJobIntervalMinute", options.UpdateJobIntervalMinute).SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]services discovery auto update job interval: {options.UpdateJobIntervalMinute} min");
            });

            return serviceHostBuilder;
        }
    }
}