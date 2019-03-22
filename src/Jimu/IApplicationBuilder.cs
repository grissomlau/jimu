using System;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Jimu
{
    public interface IApplicationBuilder<out T> where T : class
    {
        /// <summary>
        ///     register jimu framework autofac service where custom's Service cannot inject
        /// </summary>
        /// <param name="componentRegister">jimu framework autofac ContainerBuilder</param>
        /// <returns></returns>
        T AddComponent(Action<ContainerBuilder> componentRegister);


        /// <summary>
        ///     delegate will be execute in initializing host
        /// </summary>
        /// <param name="initializer">autofac IContainer</param>
        /// <returns></returns>
        T AddInitializer(Action<IContainer> initializer);

        T AddBeforeRunner(Action<IContainer> beforeRunner);
        T AddRunner(Action<IContainer> runner);

        /// <summary>
        ///     build serviceHost
        /// </summary>
        /// <returns></returns>
        IApplication Build();
    }
}