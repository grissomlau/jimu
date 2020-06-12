using Autofac;
using Jimu.Common;
using Jimu.Core.Logger.Console;
using Jimu.Logger;
using Jimu.Logger.Console;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Jimu
{
    /// <summary>
    ///     base builder for both client and server
    /// </summary>
    public abstract class ApplicationBuilderBase<T> : IApplicationBuilder<T> where T : class
    {
        /// <summary>
        ///     action will be execute in intializing: after autofac container build
        /// </summary>
        protected List<Action<IContainer>> Initializers { get; }

        /// <summary>
        ///     action will be execute in server runing
        /// </summary>
        protected List<Action<IContainer>> Runners { get; }

        protected List<Action<IContainer>> BeforeRunners { get; }

        protected List<Action<IContainer>> Disposer { get; }

        /// <summary>
        ///     action will be execute in registering: before autofac container build
        /// </summary>
        protected List<Action<ContainerBuilder>> ModuleRegisters { get; }

        protected ContainerBuilder ContainerBuilder { get; }
        /// <summary>
        /// settings for jimu app
        /// </summary>
        public IConfigurationRoot JimuAppSettings { get; }

        /// <summary>
        /// jimu setting without subfix of '.json', e.g.: JimuSetting
        /// </summary>
        protected string SettingName { get; }


        protected List<Action<ContainerBuilder>> BeforeBuilders { get; }
        protected List<Action<ContainerBuilder>> AfterLoadModules { get; }

        protected ApplicationBuilderBase(ContainerBuilder containerBuilder, string settingName)
        {
            SettingName = settingName;
            JimuAppSettings = JimuHelper.ReadSetting(settingName);
            ContainerBuilder = containerBuilder;
            ModuleRegisters = new List<Action<ContainerBuilder>>();
            Initializers = new List<Action<IContainer>>();
            Runners = new List<Action<IContainer>>();
            BeforeRunners = new List<Action<IContainer>>();
            BeforeBuilders = new List<Action<ContainerBuilder>>();
            Disposer = new List<Action<IContainer>>();
        }


        public virtual IApplication Build()
        {
            LoadModule();

            IContainer container = null;
            var host = new Application(BeforeRunners, Runners, Disposer);
            ContainerBuilder.Register(x => host).As<IApplication>().SingleInstance();
            ContainerBuilder.Register(x => container).As<IContainer>().SingleInstance();
            ContainerBuilder.RegisterType<ConsoleLoggerFactory>().As<ILoggerFactory>().SingleInstance();

            ModuleRegisters.ForEach(x => x(ContainerBuilder));
            BeforeBuilders.ForEach(x => x(ContainerBuilder));

            container = ContainerBuilder.Build();


            Initializers.ForEach(x => x(container));
            host.Container = container;
            host.JimuAppSettings = JimuAppSettings;

            return host;
        }

        public virtual T AddInitializer(Action<IContainer> initializer)
        {
            Initializers.Add(initializer);
            return this as T;
        }

        public virtual T AddRunner(Action<IContainer> runner)
        {
            Runners.Add(runner);
            return this as T;
        }

        public virtual T AddDisposer(Action<IContainer> disposer)
        {
            Disposer.Add(disposer);
            return this as T;
        }

        public virtual T AddRegister(Action<ContainerBuilder> moduleRegister)
        {
            ModuleRegisters.Add(moduleRegister);
            return this as T;
        }


        public virtual T AddBeforeRunner(Action<IContainer> beforeRunner)
        {
            BeforeRunners.Add(beforeRunner);
            return this as T;
        }

        public virtual T AddBeforeBuilder(Action<ContainerBuilder> beforeBuilder)
        {
            BeforeBuilders.Add(beforeBuilder);
            return this as T;
        }

        protected abstract void LoadModule();
    }
}