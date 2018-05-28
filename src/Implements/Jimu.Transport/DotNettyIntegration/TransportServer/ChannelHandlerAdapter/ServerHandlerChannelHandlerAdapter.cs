using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration.TransportServer.ChannelHandlerAdapter
{
    class ServerHandlerChannelHandlerAdapter : DotNetty.Transport.Channels.ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, TransportMessage> _readAction;
        private readonly ILogger _logger;

        public ServerHandlerChannelHandlerAdapter(Action<IChannelHandlerContext, TransportMessage> readAction, ILogger logger)
        {
            _readAction = readAction;
            _logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Task.Run(() =>
            {
                var msg = message as TransportMessage;
                _readAction(context, msg);
            });
        }

        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            _logger.Error($"throw exception when communicate with server:{context.Channel.RemoteAddress}", exception);
        }

    }
}
