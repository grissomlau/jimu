using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Jimu.Server.OAuth;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseJoseJwtForOAuth<T>(this IServiceHostServerBuilder serviceHostBuilder, JwtAuthorizationOptions options) where T : JimuAddress, new()
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use jose.jwt for OAuth");

                while (!container.IsRegistered<IServer>() || !container.IsRegistered<IServiceDiscovery>())
                {
                    Thread.Sleep(200);
                }
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
                var tokenRoute = new List<JimuServiceRoute> {
                    new JimuServiceRoute
                    {
                        Address = new List<JimuAddress>{
                            addr
                        },
                        ServiceDescriptor = new JimuServiceDesc
                        {
                            Id=options.GetServiceId(),
                            RoutePath = options.TokenEndpointPath
                        }
                    }
                };
                discovery.ClearServiceAsync(tokenRoute.First().ServiceDescriptor.Id).Wait();
                //discovery.SetRoutesAsync(tokenRoute);
                discovery.AddRouteAsync(tokenRoute).Wait();
            });
            return serviceHostBuilder;
        }
    }
}
