using System;
using Autofac;

namespace Jimu.Client
{
    /// <summary>
    ///     build service client
    /// </summary>
    public interface IServiceHostClientBuilder : IServiceHostBuilder
    {
        new IServiceHostClientBuilder RegisterService(Action<ContainerBuilder> serviceRegister);
        new IServiceHostClientBuilder AddInitializer(Action<IContainer> initializer);
        new IServiceHostClientBuilder AddRunner(Action<IContainer> runner);
    }
}