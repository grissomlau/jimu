using Autofac;
using Jimu.Logger;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Discovery.Implement
{
    public class DiscoveryServiceClientModule : ClientModuleBase
    {
        private readonly DiscoveryOptions _options;
        public DiscoveryServiceClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
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
                var loggerFactory = container.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(this.GetType());
                logger.Info($"[config]services discovery auto update job interval: {_options.UpdateJobIntervalMinute} min");
            }
            base.DoInit(container);
        }
    }
}
