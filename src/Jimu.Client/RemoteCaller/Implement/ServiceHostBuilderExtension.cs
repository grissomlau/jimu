using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder SetRemoteCallerRetryTimes(
            this IServiceHostClientBuilder serviceHostBuilder, int retryTimes)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().WithParameter("retryTimes", retryTimes).SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]remote service call failure retry times: {retryTimes}");
            });

            return serviceHostBuilder;
        }
    }
}