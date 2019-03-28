using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.FaultTolerant
{
    public class FaultTolerantClientModule : ClientModuleBase

    {
        private readonly FaultTolerantOptions _options;
        public FaultTolerantClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(FaultTolerantOptions).Name).Get<FaultTolerantOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null && _options.RetryTimes > 0)
            {
                componentContainerBuilder.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().WithParameter("retryTimes", _options.RetryTimes).SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null && _options.RetryTimes > 0)
            {
                var logger = container.Resolve<ILogger>();
                var caller = container.Resolve<IRemoteServiceCaller>();
                var addressSelector = container.Resolve<IAddressSelector>();
                caller.UseMiddleware<RetryCallMiddleware>(addressSelector, _options.RetryTimes, logger);
                logger.Info($"[config]remote service call failure retry times: {_options.RetryTimes}");
            }
            base.DoInit(container);
        }
    }
}
