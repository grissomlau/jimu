using Autofac;
using Jimu.Client.HealthCheck.QuartzIntegration;

namespace Jimu.Client
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseServerHealthCheck(this IServiceHostClientBuilder serviceHostBuilder, int intervalMinute)
        {
            serviceHostBuilder.RegisterService(container =>
            {
                container.RegisterType<QuartzHealthCheck>().As<IHealthCheck>().WithParameter("intervalMinute", intervalMinute).SingleInstance();
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
