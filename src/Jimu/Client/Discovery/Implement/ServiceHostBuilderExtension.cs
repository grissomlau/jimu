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

            return serviceHostBuilder;
        }
    }
}