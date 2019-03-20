using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostClientBuilderExtension
    {
        public static IApplicationClientBuilder UseHttpForTransfer(this IApplicationClientBuilder serviceHostBuilder)
        {
            serviceHostBuilder.AddInitializer((System.Action<IContainer>)(container =>
            {
                var factory = container.Resolve<ClientSenderFactory>();
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use http for transfer");

                factory.ClientSenderCreator += (global::Jimu.JimuAddress address, ref global::Jimu.Client.IClientSender client) =>
                 {
                     //if (client == null && address.GetType() == typeof(HttpAddress))
                     if (client == null && address.ServerFlag == "Http")
                     {
                         var listener = new global::Jimu.Client.ClientListener();
                         //var sender = new HttpClientSender(address, listener);
                         client = new global::Jimu.Client.HttpClientSender((global::Jimu.Client.ClientListener)listener, (global::Jimu.ILogger)logger, (global::Jimu.JimuAddress)address);
                     }
                 };
            }));
            return serviceHostBuilder;
        }
    }
}
