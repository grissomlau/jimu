using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Jimu.Server.Auth;

namespace Jimu.Server
{
    public static partial class ApplicationBuilderExtension
    {
        public static IApplicationServerBuilder UseJoseJwtForOAuth<T>(this IApplicationServerBuilder serviceHostBuilder, JwtAuthorizationOptions options) where T : JimuAddress, new()
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use jose.jwt for Auth");

                while (!container.IsRegistered<IServer>() || !container.IsRegistered<IServiceDiscovery>())
                {
                    Thread.Sleep(200);
                }
                var server = container.Resolve<IServer>();
                server.UseMiddleware<JwtAuthorizationMiddleware>(options);

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
                            RoutePath = options.TokenEndpointPath,
                             Parameters = JimuHelper.Serialize<string>(new List<JimuServiceParameterDesc>{
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "username",
                                       Format = "System.String",
                                        Name = "username",
                                         Type = "object"
                                 },
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "password",
                                       Format = "System.String",
                                        Name = "password",
                                         Type = "object"
                                 },


                             }),
                            ReturnDesc = JimuHelper.Serialize<string>( new JimuServiceReturnDesc{
                                 Comment = "Token",
                                  ReturnType = "object",
                                   ReturnFormat = "{\"access_token\":\"System.String | token\", \"expired_in\":\"System.Int32 | expired timestamp which is the number of seconds between 1970-01-01 and expired datetime\"}"
                            })
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
