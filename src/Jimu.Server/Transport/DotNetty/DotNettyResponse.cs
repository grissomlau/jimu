using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Jimu.Server.Transport.DotNetty
{
    public class DotNettyResponse : IResponse
    {
        private readonly IChannelHandlerContext _channel;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        public DotNettyResponse(IChannelHandlerContext channel, ISerializer serializer, ILogger logger)
        {
            _channel = channel;
            _serializer = serializer;
            _logger = logger;
        }
        public async Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage)
        {
            try
            {
                _logger.Info($"complete handle ： {messageId}");
                var data = _serializer.Serialize<byte[]>(new JimuTransportMsg(messageId, resultMessage));
                var buffer = Unpooled.Buffer(data.Length, data.Length);
                buffer.WriteBytes(data);
                await _channel.WriteAndFlushAsync(buffer);
            }
            catch (Exception ex)
            {
                _logger.Error("throw exception when response msg: " + messageId, ex);
            }
        }
    }
}
