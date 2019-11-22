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
            serviceHostBuilder.AddModule(containerBuilder =>
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