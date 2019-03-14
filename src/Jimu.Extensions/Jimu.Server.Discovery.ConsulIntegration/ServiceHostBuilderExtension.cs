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
        /// <param name="serviceHostBuilder"></param>
        /// <param name="consulIp">server ip</param>
        /// <param name="consulPort">server port</param>
        /// <param name="serviceGroups">which group the service register to, multiple group seperate with ','</param>
        /// <param name="serverAddress">server address</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder UseConsulForDiscovery(this IServiceHostServerBuilder serviceHostBuilder, string consulIp, int consulPort, string serviceGroups, string serverAddress)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>()
                    .WithParameter("ip", consulIp)
                    .WithParameter("port", consulPort)
                    .WithParameter("serviceGroups", serviceGroups)
                    .WithParameter("serverAddress", serverAddress)
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
               logger.Info($"[config]use consul for services discovery, consul ip: {consulIp}:{consulPort}, service group: {serviceGroups}, server address: {serverAddress} ");

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