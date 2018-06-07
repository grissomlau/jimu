using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;

namespace Jimu.Server.Transport.DotNetty
{
    class ServerHandlerChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, JimuTransportMsg> _readAction;
        private readonly ILogger _logger;

        public ServerHandlerChannelHandlerAdapter(Action<IChannelHandlerContext, JimuTransportMsg> readAction, ILogger logger)
        {
            _readAction = readAction;
            _logger = logger;
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Task.Run(() =>
            {
                var msg = message as JimuTransportMsg;
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
