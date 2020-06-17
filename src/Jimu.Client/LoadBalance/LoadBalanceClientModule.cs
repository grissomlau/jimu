using Autofac;
using Jimu.Logger;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.LoadBalance
{
    public class LoadBalanceClientModule : ClientModuleBase
    {

        private readonly LoadBalanceOptions _options;
        public LoadBalanceClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
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
                var loggerFactory = container.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(this.GetType());
                logger.Info($"[config]use {_options.LoadBalance} address selector");
            }
            base.DoInit(container);
        }

    }
}
