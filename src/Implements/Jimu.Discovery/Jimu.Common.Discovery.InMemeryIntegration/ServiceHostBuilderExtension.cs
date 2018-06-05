using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Jimu.Common.Discovery.InMemoryIntegration;

namespace Jimu.Server
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseInMemoryForDiscovery(this IServiceHostServerBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<InMemoryServiceDiscovery>().As<IServiceDiscovery>().SingleInstance();
                containerBuilder.RegisterType<InMemoryServiceDiscovery>().SingleInstance();
            });

            serviceHostBuilder.AddInitializer(async container =>
            {
                if (container.IsRegistered<IServer>())
                {
                    IServer server = container.Resolve<IServer>();
                    IServiceEntryContainer entryContainer = container.Resolve<IServiceEntryContainer>();
                    // register the method of GetRoutesAsync as microservice so that the client can caller from remote
                    entryContainer.AddServices(new[] { typeof(InMemoryServiceDiscovery) });

                    var routes = server.GetServiceRoutes();
                    var discovery = container.Resolve<IServiceDiscovery>();
                    await discovery.ClearAsync();
                    await discovery.SetRoutesAsync(routes);
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
        public static IServiceHostClientBuilder UseInMemoryForDiscovery(this IServiceHostClientBuilder serviceHostBuilder, JimuAddress address)
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var clientDiscovery = container.Resolve<IClientServiceDiscovery>();
                var remoteCaller = container.Resolve<IRemoteServiceCaller>();
                var serializer = container.Resolve<ISerializer>();
                var typeConverter = container.Resolve<ITypeConvertProvider>();
                var service = new JimuServiceRoute
                {
                    Address = new List<JimuAddress>
                    {
                        address
                    },
                    ServiceDescriptor = new JimuServiceDesc { Id = "Jimu.ServiceDiscovery.InMemory.GetRoutesDescAsync" }
                };
                clientDiscovery.AddRoutesGetter(async () =>
                {
                    var result = await remoteCaller.InvokeAsync(service, null, null);
                    if (result == null || result.HasError)
                    {
                        return null;
                    }

                    var routesDesc = (List<JimuServiceRouteDesc>)typeConverter.Convert(result.Result, typeof(List<JimuServiceRouteDesc>));
                    var routes = new List<JimuServiceRoute>();
                    foreach (var desc in routesDesc)
                    {
                        List<JimuAddress> addresses = new List<JimuAddress>(desc.AddressDescriptors.ToArray().Count());
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
            });

            return serviceHostBuilder;
        }
    }
}
