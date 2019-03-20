using Autofac;
using Jimu.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Consul;
using Jimu.Server.Discovery.ConsulIntegration;

namespace Jimu.Server
{
    public static class ServiceHostBuilderExtension
    {
        /// <summary>
        /// use consul for discovery server
        /// </summary>
        /// <returns></returns>
        public static IApplicationServerBuilder UseConsulForDiscovery(this IApplicationServerBuilder serviceHostBuilder, ConsulOptions options)
        {
            serviceHostBuilder.RegisterComponent(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>()
                    .WithParameter("ip", options.Ip)
                    .WithParameter("port", options.Port)
                    .WithParameter("serviceGroups", options.ServiceGroups)
                    .WithParameter("serverAddress", options.ServerAddress)
                    .SingleInstance();
            });

            //serviceHostBuilder.AddRunner(async container =>
            serviceHostBuilder.AddInitializer(container =>
           {
               while (!container.IsRegistered<IServer>())
               {
                   //default(SpinWait).SpinOnce();
                   Thread.Sleep(200);
               }

               var logger = container.Resolve<ILogger>();
               logger.Info($"[config]use consul for services discovery, consul ip: {options.Ip}:{options.Port}, service group: {options.ServiceGroups}, server address: {options.ServerAddress} ");

               IServer server = container.Resolve<IServer>();
               var routes = server.GetServiceRoutes();
               logger.Debug("running consul found routes count: " + routes.Count);

               try
               {
                   var discovery = container.Resolve<IServiceDiscovery>();
                   //if (!string.IsNullOrEmpty(serverAddress))
                   //{
                   //    await discovery.ClearAsync(serverAddress);
                   //}

                   discovery.ClearAsync().Wait();
                   discovery.SetRoutesAsync(routes).Wait();
               }
               catch (Exception ex)
               {
                   logger.Error($"error occurred while connecting with consul, ensure consul is running.\r\n", ex);
               }

           });
            return serviceHostBuilder;
        }



    }
}