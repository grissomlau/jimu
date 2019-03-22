using System;
using Autofac;
using Jimu.Logger;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static ApplicationClientBuilder UseToken(
            this ApplicationClientBuilder serviceHostBuilder, Func<string> getToken)
        {
            serviceHostBuilder.AddComponent(containerBuilder =>
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