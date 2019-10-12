using Autofac;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Jimu.Logger;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Transport
{
    public class TransportClientModule : ClientModuleBase
    {
        private readonly TransportOptions _options;
        public TransportClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(TransportOptions).Name).Get<TransportOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {

            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null)
            {
                var protocols = _options.Protocol.Split(',');
                foreach (var protocol in protocols)
                {
                    switch (protocol)
                    {
                        case "Netty":
                            InitNetty(container);
                            break;
                        default: break;
                    }
                }

            }
            base.DoInit(container);
        }

        private void InitNetty(IContainer container)
        {
            var factory = container.Resolve<ClientSenderFactory>();
            var logger = container.Resolve<ILogger>();
            var bootstrap = new Bootstrap();

            logger.Info($"[config]use dotnetty for transfer");

            bootstrap
                .Group(new MultithreadEventLoopGroup())
                .Channel<TcpSocketChannel>()
                .Option(ChannelOption.TcpNodelay, true)
                .Handler(new ActionChannelInitializer<IChannel>(channel =>
                {
                    var pipeline = channel.Pipeline;
                    pipeline.AddLast(new LengthFieldPrepender(4));
                    pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    pipeline.AddLast(new ReadClientMessageChannelHandlerAdapter(logger));
                    pipeline.AddLast(new ClientHandlerChannelHandlerAdapter(factory, logger));
                }));
            //AttributeKey<IClientSender> clientSenderKey = AttributeKey<IClientSender>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientSender));
            AttributeKey<ClientListener> clientListenerKey = AttributeKey<ClientListener>.ValueOf(typeof(ClientSenderFactory), nameof(ClientListener));
            //AttributeKey<EndPoint> endPointKey = AttributeKey<EndPoint>.ValueOf(typeof(DefaultTransportClientFactory), nameof(EndPoint));
            AttributeKey<string> endPointKey = AttributeKey<string>.ValueOf(typeof(ClientSenderFactory), "addresscode");
            factory.ClientSenderCreator += (JimuAddress address, ref IClientSender client) =>
            {
                //if (client == null && address.GetType().IsAssignableFrom(typeof(DotNettyAddress)))
                if (client == null && address.Protocol == "Netty")
                {
                    var ep = address.CreateEndPoint();
                    var channel = bootstrap.ConnectAsync(ep).Result;
                    var listener = new ClientListener();
                    channel.GetAttribute(clientListenerKey).Set(listener);
                    //var sender = new DotNettyClientSender(channel, logger);
                    //channel.GetAttribute(clientSenderKey).Set(sender);
                    channel.GetAttribute(endPointKey).Set($"{address.Protocol}-{address.Code}");
                    client = new DotNettyClientSender(listener, logger, channel);
                }
            };

        }

    }
}
