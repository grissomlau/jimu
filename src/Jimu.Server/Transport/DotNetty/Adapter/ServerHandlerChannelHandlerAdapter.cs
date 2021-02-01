using DotNetty.Transport.Channels;
using Jimu.Logger;
using System;
using System.Threading.Tasks;

namespace Jimu.Server.Transport.DotNetty.Adapter
{
    class ServerHandlerChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly Action<IChannelHandlerContext, JimuTransportMsg> _readAction;
        private readonly ILogger _logger;

        public ServerHandlerChannelHandlerAdapter(Action<IChannelHandlerContext, JimuTransportMsg> readAction, ILoggerFactory loggerFactory)
        {
            _readAction = readAction;
            _logger = loggerFactory.Create(this.GetType());
        }

        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            Task.Run(() =>
            {
                var msg = message as JimuTransportMsg;
                try
                {
                    _readAction(context, msg);
                }
                catch (Exception)
                {
                    _logger.Info($"handle unexpected msg: {msg}");
                }
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
