using Autofac;
using Jimu.Core.Client;
using Jimu.Core.Client.LoadBalance;

namespace Jimu
{
    public static partial class ServiceHostBuilderExtension
    {
        /// <summary>
        ///     using polling load balancing to  select server
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <returns></returns>
        public static IServiceHostClientBuilder UsePollingAddressSelector(
            this IServiceHostClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<PollingAddressSelector>().As<IAddressSelector>().SingleInstance();
            });
            return serviceHostBuilder;
        }
    }
}