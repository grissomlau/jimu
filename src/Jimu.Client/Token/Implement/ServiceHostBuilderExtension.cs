using System;
using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseToken(
            this IServiceHostClientBuilder serviceHostBuilder, Func<string> getToken)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });

            serviceHostBuilder.AddInitializer(componentRegister =>
            {
                var tokenGetter = componentRegister.Resolve<IServiceTokenGetter>();
                tokenGetter.GetToken = getToken;


                var logger = componentRegister.Resolve<ILogger>();
                logger.Info($"[config]get token has been set");
            });

            return serviceHostBuilder;
        }
    }
}