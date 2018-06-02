using System;
using System.Net;
using Autofac;
using DotNetty.Codecs;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;

namespace Jimu.Client
{
    public static class ServiceHostClientBuilderExtension
    {
        public static IServiceHostClientBuilder UseDotNettyClient(this IServiceHostClientBuilder serviceHostBuilder)
        {
            //serviceHostBuilder.RegisterService(containerBuilder =>
            //{
            //    containerBuilder.RegisterType<DotNettyTransportClientFactory>().As<ITransportClientFactory>().SingleInstance();
            //    containerBuilder.RegisterType<DotNettyTransportClient>().As<ITransportClient>();
            //});

            serviceHostBuilder.AddInitializer(container =>
            {
                var factory = container.Resolve<ITransportClientFactory>();
                var logger = container.Resolve<ILogger>();
                var serializer = container.Resolve<ISerializer>();
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(new MultithreadEventLoopGroup())
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<IChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        pipeline.AddLast(new LengthFieldPrepender(4));
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                        pipeline.AddLast(new ReadClientMessageChannelHandlerAdapter(serializer, logger));
                        pipeline.AddLast(new ClientHandlerChannelHandlerAdapter(factory, logger));
                    }));
                AttributeKey<IClientSender> clientSenderKey = AttributeKey<IClientSender>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientSender));
                AttributeKey<IClientListener> clientListenerKey = AttributeKey<IClientListener>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientListener));
                AttributeKey<EndPoint> endPointKey = AttributeKey<EndPoint>.ValueOf(typeof(DefaultTransportClientFactory), nameof(EndPoint));
                factory.ClientCreatorDelegate += (JimuAddress address, ref ITransportClient client) =>
                {
                    if (client == null && address.GetType().IsAssignableFrom(typeof(DotNettyAddress)))
                    {
                        var ep = address.CreateEndPoint();
                        var channel = bootstrap.ConnectAsync(ep).Result;
                        var listener = new DotNettyClientListener();
                        channel.GetAttribute(clientListenerKey).Set(listener);
                        var sender = new DotNettyClientSender(channel, serializer);
                        channel.GetAttribute(clientSenderKey).Set(sender);
                        channel.GetAttribute(endPointKey).Set(ep);
                        client = new DefaultTransportClient(listener, sender, logger);
                    }
                };
            });


            return serviceHostBuilder;
        }
    }
}
