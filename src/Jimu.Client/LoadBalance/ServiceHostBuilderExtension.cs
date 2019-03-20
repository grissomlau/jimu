using Autofac;
using Jimu.Client.LoadBalance;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        /// <summary>
        ///     using polling load balancing to  select server
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <returns></returns>
        public static IApplicationClientBuilder UsePollingAddressSelector(
            this IApplicationClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterComponent(containerBuilder =>
            {
                containerBuilder.RegisterType<PollingAddressSelector>().As<IAddressSelector>().SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use polling address selector");
            });
            return serviceHostBuilder;
        }
    }
}