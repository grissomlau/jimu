using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Auth
{
    public class JwtAuthClientModule : ClientModuleBase
    {
        private readonly JwtAuthorizationOptions _options;
        public JwtAuthClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(JwtAuthorizationOptions).Name).Get<JwtAuthorizationOptions>();
        }
        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;
        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use jose.jwt for Auth");

                //while (!container.IsRegistered<IRemoteServiceCaller>() || !container.IsRegistered<IClientServiceDiscovery>())
                //{
                //    Thread.Sleep(100);
                //}

                var caller = container.Resolve<IRemoteServiceCaller>();
                caller.UseMiddleware<JwtAuthorizationMiddleware>(_options);

                if (string.IsNullOrEmpty(_options.TokenEndpointPath)) return;
                var discovery = container.Resolve<IClientServiceDiscovery>();
                var addr = new JimuAddress(_options.ServiceInvokeIp, Convert.ToInt32(_options.ServiceInvokePort), _options.Protocol);
                var tokenRoute = new List<JimuServiceRoute> {
                    new JimuServiceRoute
                    {
                        Address = new List<JimuAddress>{
                            addr
                        },
                        ServiceDescriptor = new JimuServiceDesc
                        {
                            Id=_options.GetServiceId(),
                            Service = "Token",
                            HttpMethod = "POST",
                            RoutePath = JimuServiceRoute.ParseRoutePath("POST","", "",_options.TokenEndpointPath,new[]{ "username","password", "grant_type" },false),
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
                                 new JimuServiceParameterDesc
                                 {
                                      Comment = "grant_type",
                                       Format = "System.String",
                                        Name = "grant_type",
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
            }

            base.DoInit(container);
        }
    }
}
