using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Jimu.Server.Discovery;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseInServerForDiscovery(this IServiceHostServerBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<InServerServiceDiscovery>().As<IServiceDiscovery>().SingleInstance();
                containerBuilder.RegisterType<InServerServiceDiscovery>().SingleInstance();
            });

            serviceHostBuilder.AddInitializer(async container =>
            //serviceHostBuilder.AddRunner(async container =>
            {
                while (!container.IsRegistered<IServer>())
                {
                    default(SpinWait).SpinOnce();
                }
                IServer server = container.Resolve<IServer>();
                IServiceEntryContainer entryContainer = container.Resolve<IServiceEntryContainer>();
                // register the method of GetRoutesAsync as microservice so that the client can call it from remote
                entryContainer.AddServices(new[] { typeof(InServerServiceDiscovery) });

                var routes = server.GetServiceRoutes();
                var discovery = container.Resolve<IServiceDiscovery>();
                await discovery.ClearAsync();
                await discovery.SetRoutesAsync(routes);

            });
            return serviceHostBuilder;
        }



    }
}
