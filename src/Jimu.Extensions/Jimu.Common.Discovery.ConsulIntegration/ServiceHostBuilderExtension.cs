using Autofac;
using Jimu.Common.Discovery.ConsulIntegration;
using Jimu.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Consul;

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
        /// <param name="serviceCategory">server category name</param>
        /// <param name="serverAddress">server address</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder UseConsulForDiscovery(this IServiceHostServerBuilder serviceHostBuilder, string consulIp, int consulPort, string serviceCategory, string serverAddress)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsulServiceDiscovery>().As<IServiceDiscovery>()
                    .WithParameter("ip", consulIp)
                    .WithParameter("port", consulPort)
                    .WithParameter("serviceCategory", serviceCategory)
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
               logger.Info($"[config]use consul for services discovery, consul ip: {consulIp}:{consulPort}, service cateogry: {serviceCategory}, server address: {serverAddress} ");

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

namespace Jimu.Client
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseConsulForDiscovery(this IServiceHostClientBuilder serviceHostBuilder,
            string ip, int port, string serviceCategory)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use consul for services discovery, consul ip: {ip}:{port}, service cateogry: {serviceCategory}");

                var clientDiscovery = container.Resolve<IClientServiceDiscovery>();
                clientDiscovery.AddRoutesGetter(async () =>
                {
                    var consul = new ConsulClient(config => { config.Address = new Uri($"http://{ip}:{port}"); });
                    var queryResult = await consul.KV.Keys(serviceCategory);
                    var keys = queryResult.Response;
                    if (keys == null)
                    {
                        return null;
                    }

                    var routes = new List<JimuServiceRoute>();
                    foreach (var key in keys)
                    {
                        var data = (await consul.KV.Get(key)).Response?.Value;
                        if (data == null)
                        {
                            continue;
                        }

                        var descriptors = JimuHelper.Deserialize<byte[], List<JimuServiceRouteDesc>>(data);
                        if (descriptors != null && descriptors.Any())
                        {
                            foreach (var descriptor in descriptors)
                            {
                                List<JimuAddress> addresses =
                                    new List<JimuAddress>(descriptor.AddressDescriptors.ToArray().Count());
                                foreach (var addDesc in descriptor.AddressDescriptors)
                                {
                                    var addrType = Type.GetType(addDesc.Type);
                                    addresses.Add(JimuHelper.Deserialize(addDesc.Value, addrType) as JimuAddress);
                                }

                                routes.Add(new JimuServiceRoute
                                {
                                    Address = addresses,
                                    ServiceDescriptor = descriptor.ServiceDescriptor
                                });
                            }
                        }

                    }

                    return routes;
                });
            });

            return serviceHostBuilder;
        }
    }
}
