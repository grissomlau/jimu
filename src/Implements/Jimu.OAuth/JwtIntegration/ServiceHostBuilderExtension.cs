using System.Collections.Generic;
using System.Linq;
using Autofac;
using Jimu.Core.Commons.Discovery;
using Jimu.Core.Commons.Serializer;
using Jimu.Core.Protocols;
using Jimu.Core.Server;
using Jimu.Core.Server.TransportServer;
using Jimu.Server.OAuth.JwtIntegration.Middlewares;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseJwtAuthorization<T>(this IServiceHostServerBuilder serviceHostBuilder, JwtAuthorizationOptions options) where T : Address, new()
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var server = container.Resolve<IServer>();
                var serializer = container.Resolve<ISerializer>();
                server.UseMiddleware<JwtAuthorizationMiddleware>(options, serializer);

                if (string.IsNullOrEmpty(options.TokenEndpointPath)) return;
                var discovery = container.Resolve<IServiceDiscovery>();
                var addr = new T
                {
                    Ip = options.ServerIp,
                    Port = options.ServerPort
                };
                var tokenRoute = new List<ServiceRoute> {
                    new ServiceRoute
                    {
                        Address = new List<Address>{
                            addr
                        },
                        ServiceDescriptor = new ServiceDescriptor
                        {
                            Id=options.GetServiceId(),
                            RoutePath = options.TokenEndpointPath
                        }
                    }
                };
                discovery.ClearAsyncByServiceId(tokenRoute.First().ServiceDescriptor.Id);
                discovery.SetRoutesAsync(tokenRoute);
            });
            return serviceHostBuilder;
        }
    }
}
