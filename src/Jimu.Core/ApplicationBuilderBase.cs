using Autofac;
using Jimu.Common;
using Jimu.Logger;
using Jimu.Logger.Console;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;

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

        /// <summary>
        ///     action will be execute in registering: before autofac container build
        /// </summary>
        protected List<Action<ContainerBuilder>> ModuleRegisters { get; }

        protected ContainerBuilder ContainerBuilder { get; }
        /// <summary>
        /// settings for jimu app
        /// </summary>
        protected IConfigurationRoot JimuAppSettings { get; }

        /// <summary>
        /// jimu setting without subfix of '.json', e.g.: JimuSetting
        /// </summary>
        protected string SettingName { get; }

        protected ApplicationBuilderBase(ContainerBuilder containerBuilder, string settingName)
        {
            SettingName = settingName;
            JimuAppSettings = ReadSetting();
            ContainerBuilder = containerBuilder;
            ModuleRegisters = new List<Action<ContainerBuilder>>();
            Initializers = new List<Action<IContainer>>();
            Runners = new List<Action<IContainer>>();
            BeforeRunners = new List<Action<IContainer>>();
        }


        public virtual IApplication Build()
        {
            IContainer container = null;
            var host = new Application(BeforeRunners, Runners);
            ContainerBuilder.Register(x => host).As<IApplication>().SingleInstance();
            ContainerBuilder.Register(x => container).As<IContainer>().SingleInstance();
            ContainerBuilder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();

            ModuleRegisters.ForEach(x => { x(ContainerBuilder); });
            container = ContainerBuilder.Build();
            Initializers.ForEach(x => { x(container); });
            host.Container = container;

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

        public virtual T AddModule(Action<ContainerBuilder> moduleRegister)
        {
            ModuleRegisters.Add(moduleRegister);
            return this as T;
        }


        private IConfigurationRoot ReadSetting()
        {
            var jimuAppSettings = $"{SettingName}.json";
            var env = ReadJimuEvn();
            if (!string.IsNullOrEmpty(env))
            {
                jimuAppSettings = $"{SettingName}.{env}.json";
            }
            if (!File.Exists(jimuAppSettings))
            {
                throw new FileNotFoundException($"{jimuAppSettings} not found!");
            }
            return JimuHelper.GetConfig(jimuAppSettings);
        }

        private string ReadJimuEvn()
        {
            var jimuSettings = "JimuSettings.json";
            var jimuEnv = "JIMU_ENV";
            string activeProfile = "";

            if (File.Exists(jimuSettings))
            {
                var config = JimuHelper.GetConfig(jimuSettings);
                if (config != null && config["ActiveProfile"] != null)
                {
                    activeProfile = config["ActiveProfile"];
                }
            }
            if (string.IsNullOrEmpty(activeProfile?.Trim()))
            {
                var builder = new ConfigurationBuilder();
                var config = builder.AddEnvironmentVariables().Build();
                if (config != null && config[jimuEnv] != null)
                {
                    activeProfile = config[jimuEnv];
                }
            }
            return activeProfile?.Trim();
        }

        public T AddBeforeRunner(Action<IContainer> beforeRunner)
        {
            BeforeRunners.Add(beforeRunner);
            return this as T;
        }
    }
}