using Autofac;

namespace Jimu.Client
{
    public static class ServiceHostClientBuilderExtension
    {
        public static IServiceHostClientBuilder UseNetCoreHttpClient(this IServiceHostClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var factory = container.Resolve<ITransportClientFactory>();
                var logger = container.Resolve<ILogger>();
                factory.ClientCreatorDelegate += (JimuAddress address, ref ITransportClient client) =>
                 {
                     if (client == null && address.GetType() == typeof(NetCoreHttpAddress))
                     {
                         var listener = new NetCoreClientListener();
                         var sender = new NetCoreClientSender(address, listener);
                         client = new DefaultTransportClient(listener, sender, logger);
                     }
                 };
            });
            return serviceHostBuilder;
        }
    }
}
