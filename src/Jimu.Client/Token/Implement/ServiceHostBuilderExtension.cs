using Autofac;
using Jimu.Logger;
using System;

namespace Jimu.Client.Token.Implement
{
    public static partial class ServiceHostBuilderExtension
    {
        public static ApplicationClientBuilder UseToken(
            this ApplicationClientBuilder serviceHostBuilder, Func<string> getToken)
        {
            serviceHostBuilder.AddRegister(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });

            serviceHostBuilder.AddInitializer(componentRegister =>
            {
                var tokenGetter = componentRegister.Resolve<IServiceTokenGetter>();
                tokenGetter.GetToken = getToken;


                var loggerFactory = componentRegister.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(typeof(ApplicationClientBuilder));
                logger.Info($"[config]get token has been set");
            });

            return serviceHostBuilder;
        }
    }
}