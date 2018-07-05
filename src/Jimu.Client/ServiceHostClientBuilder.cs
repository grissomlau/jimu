using System;
using Autofac;

namespace Jimu.Client
{
    public class ServiceHostClientBuilder : ServiceHostBuilderBase, IServiceHostClientBuilder
    {
        public ServiceHostClientBuilder(ContainerBuilder containerBuilder) : base(containerBuilder)
        {
            this.RegisterService(cb =>
            {
                cb.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().SingleInstance();
                cb.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().SingleInstance();
                cb.RegisterType<DefaultTransportClientFactory>().As<ITransportClientFactory>().SingleInstance();
                cb.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
                cb.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });
            this.AddRunner(container =>
            {
                var clientServiceDiscovery = (ClientServiceDiscovery)container.Resolve<IClientServiceDiscovery>();
                clientServiceDiscovery?.RunInInit().Wait();

            });
        }


        public new IServiceHostClientBuilder RegisterService(Action<ContainerBuilder> serviceRegister)
        {
            return base.RegisterService(serviceRegister) as IServiceHostClientBuilder;
        }

        public new IServiceHostClientBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer) as IServiceHostClientBuilder;
        }

        public new IServiceHostClientBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner) as IServiceHostClientBuilder;
        }
    }
}