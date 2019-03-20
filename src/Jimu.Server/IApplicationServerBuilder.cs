using System;
using System.Collections.Generic;
using Autofac;

namespace Jimu.Server
{
    public interface IApplicationServerBuilder : IApplicationBuilder
    {
        new IApplicationServerBuilder RegisterComponent(Action<ContainerBuilder> componentRegister);
        new IApplicationServerBuilder AddInitializer(Action<IContainer> initializer);
        new IApplicationServerBuilder AddRunner(Action<IContainer> runner);

        /// <summary>
        /// register business autofac service where custom's Service can inject
        /// </summary>
        /// <param name="serviceRegister">custom's service autofac ContainerBuilder</param>
        /// <returns></returns>
        IApplicationServerBuilder RegisterService(Action<ContainerBuilder> serviceRegister);

        List<Action<ContainerBuilder>> ServiceRegisters { get; }
    }
}