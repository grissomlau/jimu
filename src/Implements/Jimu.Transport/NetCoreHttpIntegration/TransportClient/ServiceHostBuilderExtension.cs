using Autofac;
using Jimu.Common.Transport.NetCoreHttpIntegration;
using Jimu.Common.Transport.NetCoreHttpIntegration.TransportClient;
using Jimu.Core.Client;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Commons.Transport;
using Jimu.Core.Protocols;

namespace Jimu
{
    public static class ServiceHostClientBuilderExtension
    {
        public static IServiceHostClientBuilder UseNetCoreHttpClient(this IServiceHostClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var factory = container.Resolve<ITransportClientFactory>();
                var logger = container.Resolve<ILogger>();
                factory.ClientCreatorDelegate += (Address address, ref ITransportClient client) =>
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
