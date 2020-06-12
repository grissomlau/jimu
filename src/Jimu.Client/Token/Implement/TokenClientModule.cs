using Autofac;
using Jimu.Client.ApiGateway.Core;
using Jimu.Module;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Jimu.Client.Token.Implement
{
    public class TokenClientModule : ClientModuleBase
    {
        private readonly TokenGetterOptions _options;
        public TokenClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
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
            if (_options != null && _options.GetFrom == "HttpHeader")
            {
                var tokenGetter = container.Resolve<IServiceTokenGetter>();
                tokenGetter.GetToken = () =>
                {
                    var headers = JimuHttpContext.Current.Request.Headers["Authorization"];
                    return headers.Any() ? headers[0] : null;
                };

                var loggerFactory = container.Resolve<ILoggerFactory>();
                var logger = loggerFactory.Create(this.GetType());
                logger.Info($"[config]get token has been set");
            }
            base.DoInit(container);
        }

    }
}
