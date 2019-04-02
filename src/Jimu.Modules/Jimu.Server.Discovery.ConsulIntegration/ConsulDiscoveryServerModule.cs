using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Autofac;
using Consul;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.Discovery.ConsulIntegration
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
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>()
                                        .WithParameter("ip", _options.Ip)
                                        .WithParameter("port", _options.Port)
                                        .WithParameter("serviceGroups", _options.ServiceGroups)
                                        .WithParameter("serverAddress", _options.ServerAddress)
                                        .SingleInstance();

            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                //while (!container.IsRegistered<IServer>())
                //{
                //    //default(SpinWait).SpinOnce();
                //    Thread.Sleep(100);
                //}

                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use consul for services discovery, consul ip: {_options.Ip}:{_options.Port}, service group: {_options.ServiceGroups}, server address: {_options.ServerAddress} ");

                var serviceEntry = container.Resolve<IServiceEntryContainer>();

                serviceEntry.OnServiceLoaded += (entries) =>
                  {
                      try
                      {
                          //if (!string.IsNullOrEmpty(serverAddress))
                          //{
                          //    await discovery.ClearAsync(serverAddress);
                          //}
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
