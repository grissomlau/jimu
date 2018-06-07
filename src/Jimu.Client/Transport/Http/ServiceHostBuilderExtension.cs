using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostClientBuilderExtension
    {
        public static IServiceHostClientBuilder UseHttpForTransfer(this IServiceHostClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var factory = container.Resolve<ITransportClientFactory>();
                var logger = container.Resolve<ILogger>();
                factory.ClientCreatorDelegate += (JimuAddress address, ref ITransportClient client) =>
                 {
                     //if (client == null && address.GetType() == typeof(HttpAddress))
                     if (client == null && address.ServerFlag == "Http")
                     {
                         var listener = new HttpClientListener();
                         var sender = new HttpClientSender(address, listener);
                         client = new DefaultTransportClient(listener, sender, logger);
                     }
                 };
            });
            return serviceHostBuilder;
        }
    }
}
