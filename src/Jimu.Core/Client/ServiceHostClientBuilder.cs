using System;
using Autofac;

namespace Jimu.Core.Client
{
    public class ServiceHostClientBuilder : ServiceHostBuilderBase, IServiceHostClientBuilder
    {
        public ServiceHostClientBuilder(ContainerBuilder containerBuilder) : base(containerBuilder)
        {
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