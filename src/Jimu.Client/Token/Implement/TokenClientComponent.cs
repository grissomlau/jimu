using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Jimu.Client.ApiGateway;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Token.Implement
{
    public class TokenClientComponent : ClientComponentBase
    {
        private readonly TokenGetterOptions _options;
        public TokenClientComponent(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(TokenGetterOptions).Name).Get<TokenGetterOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                componentContainerBuilder.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null && _options.GetType == "HttpHeader")
            {
                var tokenGetter = container.Resolve<IServiceTokenGetter>();
                tokenGetter.GetToken = () =>
                {
                    var headers = JimuHttpContext.Current.Request.Headers["Authorization"];
                    return headers.Any() ? headers[0] : null;
                };

                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]get token has been set");
            }
            base.DoInit(container);
        }

    }
}
