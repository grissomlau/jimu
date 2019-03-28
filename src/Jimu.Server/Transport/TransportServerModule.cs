using Autofac;
using Jimu.Logger;
using Jimu.Server.Transport.DotNetty;
using Jimu.Server.Transport.Http;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.Transport
{
    public class TransportServerModule : ServerModuleBase
    {
        private readonly TransportOptions _options;
        public TransportServerModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(TransportOptions).Name).Get<TransportOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null)
            {
                switch (_options.Protocol)
                {

                    case "Netty":
                        componentContainerBuilder.RegisterType<DotNettyServer>().As<IServer>().WithParameter("address", new JimuAddress(_options.Ip, _options.Port, _options.Protocol)).SingleInstance();
                        break;
                    case "Http":
                        componentContainerBuilder.RegisterType<HttpServer>().As<IServer>().WithParameter("ip", _options.Ip).WithParameter("port", _options.Port).SingleInstance();
                        break;
                    default: break;
                }

            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoRun(IContainer container)
        {
            if (_options != null)
            {
                var logger = container.Resolve<ILogger>();
                switch (_options.Protocol)
                {
                    case "Netty":
                        logger.Info($"[config]use dotnetty for transfer");
                        var nettyServer = container.Resolve<IServer>();
                        nettyServer.StartAsync();
                        break;
                    case "Http":
                        logger.Info($"[config]use http for transfer");
                        var httpServer = container.Resolve<IServer>();
                        httpServer.StartAsync();
                        break;
                    default: break;
                }
            }
            base.DoInit(container);
        }
    }
}
