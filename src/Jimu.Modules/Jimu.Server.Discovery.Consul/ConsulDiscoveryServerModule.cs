using Autofac;
using Jimu.Logger;
using Jimu.Module;
using Jimu.Server.ServiceContainer;
using Jimu.Server.Transport;
using Microsoft.Extensions.Configuration;
using System;

namespace Jimu.Server.Discovery.Consul
{
    public class ConsulDiscoveryServerModule : ServerModuleBase
    {
        readonly ConsulOptions _options;
        public ConsulDiscoveryServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(ConsulOptions).Name).Get<ConsulOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null && _options.Enable)
            {
                componentContainerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>()
                                        .WithParameter("ip", _options.Ip)
                                        .WithParameter("port", Convert.ToInt32(_options.Port))
                                        .WithParameter("serviceGroups", _options.ServiceGroups)
                                        .WithParameter("serverAddress", $"{_options.ServiceInvokeIp}:{_options.ServiceInvokePort}")
                                        .SingleInstance();

            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null && _options.Enable)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use consul for services discovery, consul ip: {_options.Ip}:{_options.Port}, service group: {_options.ServiceGroups}, server address: {_options.ServiceInvokeIp}:{_options.ServiceInvokePort} ");

                var serviceEntry = container.Resolve<IServiceEntryContainer>();

                serviceEntry.OnServiceLoaded += (entries) =>
                  {
                      try
                      {
                          var discovery = container.Resolve<IServiceDiscovery>();
                          var server = container.Resolve<IServer>();
                          var routes = server.GetServiceRoutes();
                          logger.Debug("running consul found routes count: " + routes.Count);
                          discovery.ClearAsync().Wait();
                          discovery.SetRoutesAsync(routes).Wait();
                      }
                      catch (Exception ex)
                      {
                          logger.Error($"error occurred while connecting with consul, ensure consul is running.\r\n", ex);
                      }
                  };
            }
            base.DoInit(container);
        }


    }
}
