using System;
using System.Collections.Generic;
using Autofac;
using Jimu.Logger;

namespace Jimu
{
    /// <summary>
    ///     base builder for both client and server
    /// </summary>
    public abstract class ServiceHostBuilderBase : IServiceHostBuilder
    {
        /// <summary>
        ///     action will be execute in intializing: after autofac container build
        /// </summary>
        private readonly List<Action<IContainer>> _initializers;

        /// <summary>
        ///     action will be execute in server runing
        /// </summary>
        private readonly List<Action<IContainer>> _runners;

        /// <summary>
        ///     action will be execute in registering: before autofac container build
        /// </summary>
        private readonly List<Action<ContainerBuilder>> _serviceRegisters;

        private readonly ContainerBuilder _containerBuilder;

        protected ServiceHostBuilderBase(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
            _serviceRegisters = new List<Action<ContainerBuilder>>();
            _initializers = new List<Action<IContainer>>();
            _runners = new List<Action<IContainer>>();
        }

        public IServiceHost Build()
        {
            IContainer container = null;
            var host = new ServiceHost(_runners);
            _containerBuilder.Register(x => host).As<IServiceHost>().SingleInstance();
            _containerBuilder.Register(x => container).As<IContainer>().SingleInstance();
            //_containerBuilder.RegisterType<Log4netLogger>().As<ILogger>().SingleInstance();
            _containerBuilder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();


            _serviceRegisters.ForEach(x => { x(_containerBuilder); });

            container = _containerBuilder.Build();
            _initializers.ForEach(x => { x(container); });

            host.Container = container;

            return host;
        }

        public IServiceHostBuilder AddInitializer(Action<IContainer> initializer)
        {
            _initializers.Add(initializer);
            return this;
        }

        public IServiceHostBuilder AddRunner(Action<IContainer> runner)
        {
            _runners.Add(runner);
            return this;
        }

        public IServiceHostBuilder RegisterService(Action<ContainerBuilder> serviceRegister)
        {
            _serviceRegisters.Add(serviceRegister);
            return this;
        }
    }
}