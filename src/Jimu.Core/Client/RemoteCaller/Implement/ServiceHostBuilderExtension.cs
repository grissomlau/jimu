using System;
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

            return serviceHostBuilder;
        }
    }
}