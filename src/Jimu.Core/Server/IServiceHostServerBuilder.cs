using System;
using Autofac;

namespace Jimu.Core.Server
{
    public interface IServiceHostServerBuilder : IServiceHostBuilder
    {
        new IServiceHostServerBuilder RegisterService(Action<ContainerBuilder> serviceRegister);
        new IServiceHostServerBuilder AddInitializer(Action<IContainer> initializer);
        new IServiceHostServerBuilder AddRunner(Action<IContainer> runner);
    }
}