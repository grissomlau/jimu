using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder SetDiscoveryAutoUpdateJobInterval(
            this IServiceHostClientBuilder serviceHostBuilder, int updateJobIntervalMinute)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().WithParameter("updateJobIntervalMinute", updateJobIntervalMinute).SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]services discovery auto update job interval: {updateJobIntervalMinute} min");
            });

            return serviceHostBuilder;
        }
    }
}