using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.HealthCheck
{
    public class HealthCheckClientModule : ClientModuleBase
    {
        private readonly HealthCheckOptions _options;
        public HealthCheckClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(HealthCheckOptions).Name).Get<HealthCheckOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<QuartzHealthCheck>().As<IHealthCheck>().WithParameter("intervalMinute", _options.IntervalMinute).SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var healthCheck = container.Resolve<IHealthCheck>();
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use server health check, checked job interval: {_options.IntervalMinute} min");
                healthCheck.RunAsync();
            }
            base.DoInit(container);
        }
    }
}
