using Autofac;
using Jimu.Client.FaultTolerant;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder SetRemoteCallerRetryTimes(
            this IServiceHostClientBuilder serviceHostBuilder, FaultTolerantOptions options)
        {
            if (options.RetryTimes > 0)
            {
                serviceHostBuilder.RegisterService(containerBuilder =>
                {
                    containerBuilder.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().WithParameter("retryTimes", options.RetryTimes).SingleInstance();
                });
                serviceHostBuilder.AddInitializer(container =>
                {
                    var logger = container.Resolve<ILogger>();
                    var caller = container.Resolve<IRemoteServiceCaller>();
                    var addressSelector = container.Resolve<IAddressSelector>();
                    caller.UseMiddleware<RetryCallMiddleware>(addressSelector, options.RetryTimes, logger);
                    logger.Info($"[config]remote service call failure retry times: {options.RetryTimes}");
                });
            }

            return serviceHostBuilder;
        }
    }
}