using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.ServiceContainer.Implement
{
    public class ServiceLoaderServerModule : ServerModuleBase
    {
        public ServiceOptions _options;
        public ServiceLoaderServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(ServiceOptions).Name).Get<ServiceOptions>();

        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<ServiceEntryContainer>().As<IServiceEntryContainer>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoBeforeRun(IContainer container)
        {
            if (_options != null)
            {
                var serviceEntryContainer = container.Resolve<IServiceEntryContainer>();
                var logger = container.Resolve<ILogger>();
                ServicesLoader servicesLoader = new ServicesLoader(serviceEntryContainer, logger, _options);
                servicesLoader.LoadServices();
            }
            base.DoInit(container);
        }

    }
}
