using System;
using Autofac;
using Jimu.Core.Client;
using Jimu.Core.Client.RemoteInvoker;
using Jimu.Core.Server.ServiceContainer;

namespace Jimu
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseRemoteServiceInvoker(
            this IServiceHostClientBuilder serviceHostBuilder, Func<string> getToken)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<RemoteServiceInvoker>().As<IRemoteServiceInvoker>().SingleInstance();
                containerBuilder.RegisterType<TypeConvertProvider>().As<ITypeConvertProvider>().SingleInstance();
                containerBuilder.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });

            serviceHostBuilder.AddInitializer(componentRegister =>
            {
                var tokenGetter = componentRegister.Resolve<IServiceTokenGetter>();
                tokenGetter.GetToken = getToken;
            });

            return serviceHostBuilder;
        }
    }
}