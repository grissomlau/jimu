using System;
using Autofac;

namespace Jimu.Core.Server
{
    public class ServiceHostServerBuilder : ServiceHostBuilderBase, IServiceHostServerBuilder
    {
        public ServiceHostServerBuilder(ContainerBuilder containerBuilder) : base(containerBuilder)
        {
        }


        public new IServiceHostServerBuilder RegisterService(Action<ContainerBuilder> serviceRegister)
        {
            return base.RegisterService(serviceRegister) as IServiceHostServerBuilder;
        }

        public new IServiceHostServerBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer) as IServiceHostServerBuilder;
        }

        public new IServiceHostServerBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner) as IServiceHostServerBuilder;
        }
    }
}