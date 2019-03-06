using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Consul;

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
