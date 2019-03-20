using System;
using Autofac;

namespace Jimu
{
    public interface IApplicationBuilder
    {
        /// <summary>
        ///     register jimu framework autofac service where custom's Service cannot inject
        /// </summary>
        /// <param name="componentRegister">jimu framework autofac ContainerBuilder</param>
        /// <returns></returns>
        IApplicationBuilder RegisterComponent(Action<ContainerBuilder> componentRegister);


        /// <summary>
        ///     delegate will be execute in initializing host
        /// </summary>
        /// <param name="initializer">autofac IContainer</param>
        /// <returns></returns>
        IApplicationBuilder AddInitializer(Action<IContainer> initializer);

        IApplicationBuilder AddRunner(Action<IContainer> runner);

        /// <summary>
        ///     build serviceHost
        /// </summary>
        /// <returns></returns>
        IApplication Build();
    }
}