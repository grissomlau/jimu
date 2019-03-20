using System;
using System.Collections.Generic;
using Autofac;
using Jimu.Logger;

namespace Jimu
{
    /// <summary>
    ///     base builder for both client and server
    /// </summary>
    public abstract class ApplicationBuilderBase : IApplicationBuilder
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
        private readonly List<Action<ContainerBuilder>> _componentRegisters;

        private readonly ContainerBuilder _containerBuilder;

        protected ApplicationBuilderBase(ContainerBuilder containerBuilder)
        {
            _containerBuilder = containerBuilder;
            _componentRegisters = new List<Action<ContainerBuilder>>();
            _initializers = new List<Action<IContainer>>();
            _runners = new List<Action<IContainer>>();
        }

        public IApplication Build()
        {
            IContainer container = null;
            var host = new Application(_runners);
            _containerBuilder.Register(x => host).As<IApplication>().SingleInstance();
            _containerBuilder.Register(x => container).As<IContainer>().SingleInstance();
            //_containerBuilder.RegisterType<Log4netLogger>().As<ILogger>().SingleInstance();
            _containerBuilder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();


            _componentRegisters.ForEach(x => { x(_containerBuilder); });

            container = _containerBuilder.Build();
            _initializers.ForEach(x => { x(container); });

            host.Container = container;

            return host;
        }

        public IApplicationBuilder AddInitializer(Action<IContainer> initializer)
        {
            _initializers.Add(initializer);
            return this;
        }

        public IApplicationBuilder AddRunner(Action<IContainer> runner)
        {
            _runners.Add(runner);
            return this;
        }

        public IApplicationBuilder RegisterComponent(Action<ContainerBuilder> componentRegister)
        {
            _componentRegisters.Add(componentRegister);
            return this;
        }
    }
}