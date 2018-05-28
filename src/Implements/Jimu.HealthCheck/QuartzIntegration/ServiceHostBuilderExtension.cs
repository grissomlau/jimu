using Autofac;
using Jimu.Client.HealthCheck.QuartzIntegration;
using Jimu.Core.Client;
using Jimu.Core.Client.HealthCheck;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseServerHealthCheck(this IServiceHostClientBuilder serviceHostBuilder, int intervalMinute)
        {
            serviceHostBuilder.RegisterService(container =>
            {
                container.RegisterType<HealthCheck>().As<IHealthCheck>().WithParameter("intervalMinute", intervalMinute).SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var healthCheck = container.Resolve<IHealthCheck>();
                healthCheck.RunAsync();
            });
            return serviceHostBuilder;
        }
    }
}
