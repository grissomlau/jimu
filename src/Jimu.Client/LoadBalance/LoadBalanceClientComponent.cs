using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.LoadBalance
{
    public class LoadBalanceClientComponent : ClientComponentBase
    {

        private readonly LoadBalanceOptions _options;
        public LoadBalanceClientComponent(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(LoadBalanceOptions).Name).Get<LoadBalanceOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null && _options.LoadBalance == "Polling")
            {
                componentContainerBuilder.RegisterType<PollingAddressSelector>().As<IAddressSelector>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null && !string.IsNullOrEmpty(_options.LoadBalance))
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use {_options.LoadBalance} address selector");
            }
            base.DoInit(container);
        }

    }
}
