using System;
using Autofac;

namespace Jimu.Client
{
    /// <summary>
    ///     build service client
    /// </summary>
    public interface IApplicationClientBuilder : IApplicationBuilder
    {
        new IApplicationClientBuilder RegisterComponent(Action<ContainerBuilder> serviceRegister);
        new IApplicationClientBuilder AddInitializer(Action<IContainer> initializer);
        new IApplicationClientBuilder AddRunner(Action<IContainer> runner);
    }
}