using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Common;
using Jimu.Logger;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Server.Transport.DotNetty
{
    public class DotNettyResponse : IResponse
    {
        private readonly IChannelHandlerContext _channel;
        private readonly ILogger _logger;
        public DotNettyResponse(IChannelHandlerContext channel, ILogger logger)
        {
            _channel = channel;
            _logger = logger;
        }
        public async Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage)
        {
            try
            {
                _logger.Debug($"finish handling msg: {messageId}");
                var data = JimuHelper.Serialize<byte[]>(new JimuTransportMsg(messageId, resultMessage));
                if (data.Length < 102400)
                {
                    _logger.Debug($"resp msg is: {Encoding.UTF8.GetString(data)}");
                }
                else
                {
                    _logger.Debug($"resp msg is (bigger than 100k, we don't show it)");
                }

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
