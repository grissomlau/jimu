using Autofac;
using System;

namespace Jimu
{
    public interface IApplicationBuilder<out T> where T : class
    {
        /// <summary>
        /// step 1: register jimu framework autofac service where custom's Service cannot inject
        /// </summary>
        /// <param name="moduleRegister">jimu framework autofac ContainerBuilder</param>
        /// <returns></returns>
        T AddRegister(Action<ContainerBuilder> moduleRegister);
        /// <summary>
        /// step 2: after all module register
        /// </summary>
        /// <param name="beforeBuilder"></param>
        /// <returns></returns>
        T AddBeforeBuilder(Action<ContainerBuilder> beforeBuilder);

        /// <summary>
        /// step 3: build serviceHost
        /// </summary>
        /// <returns></returns>
        IApplication Build();

        /// <summary>
        /// step 4: after build, delegate will be execute in initializing host
        /// </summary>
        /// <param name="initializer">autofac IContainer</param>
        /// <returns></returns>
        T AddInitializer(Action<IContainer> initializer);

        /// <summary>
        /// step 5: will run
        /// </summary>
        /// <param name="beforeRunner"></param>
        /// <returns></returns>
        T AddBeforeRunner(Action<IContainer> beforeRunner);
        /// <summary>
        /// step 6: run the application
        /// </summary>
        /// <param name="runner"></param>
        /// <returns></returns>
        T AddRunner(Action<IContainer> runner);


    }
}