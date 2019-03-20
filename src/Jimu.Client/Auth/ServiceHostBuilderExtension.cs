using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Jimu.Client.Auth;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IApplicationClientBuilder UseJoseJwtForOAuth<T>(this IApplicationClientBuilder serviceHostBuilder, JwtAuthorizationOptions options) where T : JimuAddress, new()
        {
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use jose.jwt for Auth");

                while (!container.IsRegistered<IRemoteServiceCaller>() || !container.IsRegistered<IClientServiceDiscovery>())
                {
                    Thread.Sleep(200);
                }

                var caller = container.Resolve<IRemoteServiceCaller>();
                caller.UseMiddleware<JwtAuthorizationMiddleware>(options);

                if (string.IsNullOrEmpty(options.TokenEndpointPath)) return;
                var discovery = container.Resolve<IClientServiceDiscovery>();
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
                            RoutePath = JimuServiceRoute.ParseRoutePath("", "",options.TokenEndpointPath,new[]{ "username","password"},false),
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
                discovery.AddRoutesGetter(() =>
                {
                    return Task.FromResult(tokenRoute);
                });
            });
            return serviceHostBuilder;
        }
    }
}
