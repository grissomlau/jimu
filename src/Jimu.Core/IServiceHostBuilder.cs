using System;
using Autofac;

namespace Jimu.Core
{
    public interface IServiceHostBuilder
    {
        /// <summary>
        ///     register autofac service
        /// </summary>
        /// <param name="serviceRegister">autofac ContainerBuilder</param>
        /// <returns></returns>
        IServiceHostBuilder RegisterService(Action<ContainerBuilder> serviceRegister);

        /// <summary>
        ///     delegate will be exute in initializing host
        /// </summary>
        /// <param name="initializer">autofac IContainer</param>
        /// <returns></returns>
        IServiceHostBuilder AddInitializer(Action<IContainer> initializer);

        IServiceHostBuilder AddRunner(Action<IContainer> runner);

        /// <summary>
        ///     buid serviceHost
        /// </summary>
        /// <returns></returns>
        IServiceHost Build();
    }
}