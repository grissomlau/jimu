using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Discovery.Implement
{
    public class DiscoveryServiceClientComponent : ClientComponentBase
    {
        private readonly DiscoveryOptions _options;
        public DiscoveryServiceClientComponent(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(DiscoveryOptions).Name).Get<DiscoveryOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {

                componentContainerBuilder.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().WithParameter("updateJobIntervalMinute", _options.UpdateJobIntervalMinute).SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]services discovery auto update job interval: {_options.UpdateJobIntervalMinute} min");
            }
            base.DoInit(container);
        }
    }
}
