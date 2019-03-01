using System;
using Autofac;

namespace Jimu
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
        ///     delegate will be execute in initializing host
        /// </summary>
        /// <param name="initializer">autofac IContainer</param>
        /// <returns></returns>
        IServiceHostBuilder AddInitializer(Action<IContainer> initializer);

        IServiceHostBuilder AddRunner(Action<IContainer> runner);

        /// <summary>
        ///     build serviceHost
        /// </summary>
        /// <returns></returns>
        IServiceHost Build();
    }
}