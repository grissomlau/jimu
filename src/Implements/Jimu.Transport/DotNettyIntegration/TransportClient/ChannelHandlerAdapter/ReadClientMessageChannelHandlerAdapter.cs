using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Commons.Serializer;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration.TransportClient.ChannelHandlerAdapter
{
    class ReadClientMessageChannelHandlerAdapter : DotNetty.Transport.Channels.ChannelHandlerAdapter
    {
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        public ReadClientMessageChannelHandlerAdapter(ISerializer serializer, ILogger logger)
        {
            _serializer = serializer;
            _logger = logger;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            try
            {
                byte[] data = new byte[buffer.ReadableBytes];
                buffer.GetBytes(buffer.ReaderIndex, data);

                _logger.Info("Read Message is ");
                _logger.Info(Encoding.UTF8.GetString(data));
                var convertedMsg = _serializer.Deserialize<byte[], TransportMessage>(data);
                if (convertedMsg.IsInvokeMessage())
                {
                    convertedMsg.Content = _serializer.Deserialize<string, RemoteInvokeMessage>(convertedMsg.Content.ToString());
                }
                else if (convertedMsg.IsInvokeResultMessage())
                {
                    convertedMsg.Content = _serializer.Deserialize<string, RemoteInvokeResultMessage>(convertedMsg.Content.ToString());
                }
                context.FireChannelRead(convertedMsg);
            }
            finally
            {
                buffer.Release();
            }
            //base.ChannelRead(context, message);
        }
    }
}
