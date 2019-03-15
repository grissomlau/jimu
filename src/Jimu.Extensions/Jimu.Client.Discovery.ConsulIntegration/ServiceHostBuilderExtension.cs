using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Consul;
using Jimu.Client.Discovery.ConsulIntegration;

namespace Jimu.Client
{
    public static class ServiceHostBuilderExtension
    {
        /// <summary>
        /// discovery service
        /// </summary>
        /// <param name="serviceGroups"></param>
        /// <returns></returns>
        public static IServiceHostClientBuilder UseConsulForDiscovery(this IServiceHostClientBuilder serviceHostBuilder, ConsulOptions options)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use consul for services discovery, consul ip: {options.Ip}:{options.Port}, service cateogry: {options.ServiceGroups}");

                var clientDiscovery = container.Resolve<IClientServiceDiscovery>();
                clientDiscovery.AddRoutesGetter(async () =>
                {
                    var consul = new ConsulClient(config => { config.Address = new Uri($"http://{options.Ip}:{options.Port}"); });
                    HashSet<string> keyset = new HashSet<string>();
                    foreach (var group in options.ServiceGroups.Split(','))
                    {
                        if (string.IsNullOrEmpty(group)) continue;
                        var queryResult = await consul.KV.Keys(group);
                        if (queryResult == null || queryResult.Response == null) continue;

                        foreach (var key in queryResult.Response)
                        {
                            keyset.Add(key);
                        }
                    }
                    if (!keyset.Any())
                    {
                        return null;
                    }

                    var routes = new List<JimuServiceRoute>();
                    foreach (var key in keyset)
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
