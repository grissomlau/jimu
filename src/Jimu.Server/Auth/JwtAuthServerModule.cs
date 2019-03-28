using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.Auth
{
    public class JwtAuthServerModule : ServerModuleBase
    {
        private readonly JwtAuthorizationOptions _options;
        public JwtAuthServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(JwtAuthorizationOptions).Name).Get<JwtAuthorizationOptions>();
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use jose.jwt for Auth");

                while (!container.IsRegistered<IServer>() || !container.IsRegistered<IServiceDiscovery>())
                {
                    Thread.Sleep(200);
                }
                var server = container.Resolve<IServer>();
                server.UseMiddleware<JwtAuthorizationMiddleware>(_options, container);

                if (string.IsNullOrEmpty(_options.TokenEndpointPath)) return;
                var discovery = container.Resolve<IServiceDiscovery>();
                var addr = new JimuAddress(_options.ServerIp, _options.ServerPort, _options.Protocol);
                var tokenRoute =
                    new JimuServiceRoute
                    {
                        Address = new List<JimuAddress>{
                            addr
                        },
                        ServiceDescriptor = new JimuServiceDesc
                        {
                            Id = _options.GetServiceId(),
                            RoutePath = JimuServiceRoute.ParseRoutePath("", "", _options.TokenEndpointPath, new[] { "username", "password" }, false),
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
                            ReturnDesc = JimuHelper.Serialize<string>(new JimuServiceReturnDesc
                            {
                                Comment = "Token",
                                ReturnType = "object",
                                ReturnFormat = "{\"access_token\":\"System.String | token\", \"expired_in\":\"System.Int32 | expired timestamp which is the number of seconds between 1970-01-01 and expired datetime\"}"
                            })
                        }
                    };
                //discovery.ClearServiceAsync(tokenRoute.First().ServiceDescriptor.Id).Wait();
                ////discovery.SetRoutesAsync(tokenRoute);
                //discovery.AddRouteAsync(tokenRoute).Wait();
                discovery.OnBeforeSetRoutes += (routes) =>
                {
                    routes.Add(tokenRoute);
                };
            }

            base.DoInit(container);
        }
    }
}
