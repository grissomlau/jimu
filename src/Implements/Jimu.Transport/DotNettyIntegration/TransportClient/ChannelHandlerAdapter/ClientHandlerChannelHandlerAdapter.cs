using System.Net;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Commons.Transport;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration.TransportClient.ChannelHandlerAdapter
{
    class ClientHandlerChannelHandlerAdapter : DotNetty.Transport.Channels.ChannelHandlerAdapter
    {
        private readonly ITransportClientFactory _factory;
        private readonly ILogger _logger;


        public ClientHandlerChannelHandlerAdapter(ITransportClientFactory factory
            , ILogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _factory.Clients.TryRemove(context.Channel.GetAttribute(AttributeKey<EndPoint>.ValueOf(typeof(DefaultTransportClientFactory), nameof(EndPoint))).Get(), out var value);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as TransportMessage;

            var listener = context.Channel.GetAttribute(AttributeKey<IClientListener>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientListener))).Get();
            var sender = context.Channel.GetAttribute(AttributeKey<IClientSender>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientSender))).Get();

            listener.Received(sender, msg);
        }

    }
}
