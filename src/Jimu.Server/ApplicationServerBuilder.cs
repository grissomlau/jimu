using System;
using System.Collections.Generic;
using Autofac;

namespace Jimu.Server
{
    public class ApplicationServerBuilder : ApplicationBuilderBase, IApplicationServerBuilder
    {

        public List<Action<ContainerBuilder>> ServiceRegisters => new List<Action<ContainerBuilder>>();

        public ApplicationServerBuilder(ContainerBuilder containerBuilder) : base(containerBuilder)
        {
        }


        public new IApplicationServerBuilder RegisterComponent(Action<ContainerBuilder> serviceRegister)
        {
            return base.RegisterComponent(serviceRegister) as IApplicationServerBuilder;
        }

        public new IApplicationServerBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer) as IApplicationServerBuilder;
        }

        public new IApplicationServerBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner) as IApplicationServerBuilder;
        }

        public IApplicationServerBuilder RegisterService(Action<ContainerBuilder> serviceRegister)
        {
            ServiceRegisters.Add(serviceRegister);
            return this;
        }
    }
}