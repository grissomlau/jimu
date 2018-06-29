using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseInServerForDiscovery(this IServiceHostClientBuilder serviceHostBuilder, params JimuAddress[] address)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var clientDiscovery = container.Resolve<IClientServiceDiscovery>();
                var remoteCaller = container.Resolve<IRemoteServiceCaller>();
                var serializer = container.Resolve<ISerializer>();
                var typeConverter = container.Resolve<ITypeConvertProvider>();

                foreach (var addr in address)
                {
                    var service = new JimuServiceRoute
                    {
                        Address = new List<JimuAddress>
                        {
                            addr
                        },
                        ServiceDescriptor = new JimuServiceDesc { Id = "Jimu.ServiceDiscovery.InServer.GetRoutesDescAsync" }
                    };
                    clientDiscovery.AddRoutesGetter(async () =>
                    {
                        var result = await remoteCaller.InvokeAsync(service, null, null);
                        if (result == null || result.HasError)
                        {
                            return null;
                        }

                        var routesDesc =
                            (List<JimuServiceRouteDesc>)typeConverter.Convert(result.Result,
                                typeof(List<JimuServiceRouteDesc>));
                        var routes = new List<JimuServiceRoute>();
                        foreach (var desc in routesDesc)
                        {
                            List<JimuAddress> addresses =
                                new List<JimuAddress>(desc.AddressDescriptors.ToArray().Count());
                            foreach (var addDesc in desc.AddressDescriptors)
                            {
                                var addrType = Type.GetType(addDesc.Type);
                                addresses.Add(serializer.Deserialize(addDesc.Value, addrType) as JimuAddress);
                            }

                            routes.Add(new JimuServiceRoute()
                            {
                                ServiceDescriptor = desc.ServiceDescriptor,
                                Address = addresses
                            });
                        }

                        return routes;
                    });
                }
            });

            return serviceHostBuilder;
        }
    }
}
