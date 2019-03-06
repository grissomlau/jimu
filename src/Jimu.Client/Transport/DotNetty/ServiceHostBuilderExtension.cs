using System.Net;
using Autofac;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Jimu.Client
{
    public static partial class ServiceHostClientBuilderExtension
    {
        public static IServiceHostClientBuilder UseDotNettyForTransfer(this IServiceHostClientBuilder serviceHostBuilder)
        {

            serviceHostBuilder.AddInitializer(container =>
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
                    if (client == null && address.ServerFlag == "DotNetty")
                    {
                        var ep = address.CreateEndPoint();
                        var channel = bootstrap.ConnectAsync(ep).Result;
                        var listener = new ClientListener();
                        channel.GetAttribute(clientListenerKey).Set(listener);
                        //var sender = new DotNettyClientSender(channel, logger);
                        //channel.GetAttribute(clientSenderKey).Set(sender);
                        channel.GetAttribute(endPointKey).Set($"{address.ServerFlag}-{address.Code}");
                        client = new DotNettyClientSender(listener, logger, channel);
                    }
                };
            });


            return serviceHostBuilder;
        }
    }
}
