using System.Net;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace Jimu.Client
{
    class ClientHandlerChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ClientSenderFactory _factory;
        private readonly ILogger _logger;


        public ClientHandlerChannelHandlerAdapter(ClientSenderFactory factory
            , ILogger logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            _factory.ClientSenders.TryRemove(context.Channel.GetAttribute(AttributeKey<string>.ValueOf(typeof(ClientSenderFactory), "addresscode")).Get(), out _);
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as JimuTransportMsg;

            var listener = context.Channel.GetAttribute(AttributeKey<ClientListener>.ValueOf(typeof(ClientSenderFactory), nameof(ClientListener))).Get();
            //var sender = context.Channel.GetAttribute(AttributeKey<IClientSender>.ValueOf(typeof(DefaultTransportClientFactory), nameof(IClientSender))).Get();

            //listener.Received(sender, msg);
            listener.Received(msg);
        }

    }
}
