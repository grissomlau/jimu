using Autofac;
using Jimu.Common;
using Jimu.Logger;
using Jimu.Module;
using Jimu.Server.Auth.Middlewares;
using Jimu.Server.Discovery;
using Jimu.Server.Transport;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace Jimu.Server.Auth
{
    public class JwtAuthServerModule : ServerModuleBase
    {
        private readonly JwtAuthorizationOptions _options;
        public JwtAuthServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(JwtAuthorizationOptions).Name).Get<JwtAuthorizationOptions>();
        }
        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;
        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                
                var loggerFactory = container.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(this.GetType());
                logger.Info($"[config]use jose.jwt for Auth");

                //while (!container.IsRegistered<IServer>() || !container.IsRegistered<IServiceDiscovery>())
                //{
                //    Thread.Sleep(200);
                //}
                var server = container.Resolve<IServer>();
                server.UseMiddleware<JwtAuthorizationMiddleware>(_options, container);

                if (string.IsNullOrEmpty(_options.TokenEndpointPath)) return;
                var discovery = container.Resolve<IServiceDiscovery>();
                var addr = new JimuAddress(_options.ServiceInvokeIp, Convert.ToInt32(_options.ServiceInvokePort), _options.Protocol);
                var tokenRoute =
                    new JimuServiceRoute
                    {
                        Address = new List<JimuAddress>{
                            addr
                        },
                        ServiceDescriptor = new JimuServiceDesc
                        {
                            Id = _options.GetServiceId(),
                            Service = "Token",
                            HttpMethod = "POST",
                            AllowAnonymous = true,
                            RoutePath = JimuServiceRoute.ParseRoutePath("POST", "", "", _options.TokenEndpointPath, new[] { "username", "password", "grant_type" }, false),
                            Parameters = JimuHelper.Serialize<string>(new List<JimuServiceParameterDesc>{
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "username",
                                        Name = "username",
                                         Type = "System.String"
                                 },
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "password",
                                        Name = "password",
                                         Type = "System.String"
                                 },
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "grant_type",
                                       Default = "password",
                                        Name = "grant_type",
                                         Type = "System.String",

                                 },


                             }),
                            ReturnDesc = JimuHelper.Serialize<string>(new JimuServiceReturnDesc
                            {
                                Comment = "Token",
                                ReturnType = "object",
                                Properties = new List<JimuServiceParameterDesc>
                                 {
                                     new JimuServiceParameterDesc{
                                          Comment = "token",
                                           Name = "access_token",
                                            Type = "System.String"
                                     },
                                     new JimuServiceParameterDesc{
                                          Comment = "expired timestamp which is the number of seconds between 1970-01-01 and expired datetime",
                                           Name = "expired_in",
                                            Type = "System.Int32"
                                     }
                                }
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
