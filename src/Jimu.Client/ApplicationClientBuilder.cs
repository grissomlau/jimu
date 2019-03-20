using System;
using Autofac;

namespace Jimu.Client
{
    public class ApplicationClientBuilder : ApplicationBuilderBase, IApplicationClientBuilder
    {
        public ApplicationClientBuilder(ContainerBuilder containerBuilder) : base(containerBuilder)
        {
            this.RegisterComponent(cb =>
            {
                cb.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().SingleInstance();
                cb.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().SingleInstance();
                cb.RegisterType<ClientSenderFactory>().AsSelf().SingleInstance();
                cb.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
                cb.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });
            this.AddRunner(container =>
            {
                var clientServiceDiscovery = (ClientServiceDiscovery)container.Resolve<IClientServiceDiscovery>();
                clientServiceDiscovery?.RunInInit().Wait();

            });
        }


        public new IApplicationClientBuilder RegisterComponent(Action<ContainerBuilder> serviceRegister)
        {
            return base.RegisterComponent(serviceRegister) as IApplicationClientBuilder;
        }

        public new IApplicationClientBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer) as IApplicationClientBuilder;
        }

        public new IApplicationClientBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner) as IApplicationClientBuilder;
        }
    }
}