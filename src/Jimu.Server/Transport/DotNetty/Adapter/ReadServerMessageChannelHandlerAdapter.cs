using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Common;
using Jimu.Logger;
using System;
using System.Text;

namespace Jimu.Server.Transport.DotNetty.Adapter
{
    class ReadServerMessageChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        public ReadServerMessageChannelHandlerAdapter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(this.GetType());
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            try
            {
                byte[] data = new byte[buffer.ReadableBytes];
                buffer.GetBytes(buffer.ReaderIndex, data);

                if (data.Length < 102400)
                {
                    _logger.Debug($"recevied msg is: {Encoding.UTF8.GetString(data)}");
                }
                else
                {
                    _logger.Debug($"recevied msg is (bigger than 100k, we don't show it)");
                }
                var convertedMsg = JimuHelper.Deserialize<byte[], JimuTransportMsg>(data);
                context.FireChannelRead(convertedMsg);
                //context.FireChannelRead(data);
            }
            catch (Exception ex)
            {
                _logger.Debug($"Deserialize msg failure");
                context.WriteAndFlushAsync(Encoding.UTF8.GetBytes($"failure, {ex.ToStackTraceString()}"));
            }
            finally
            {
                buffer.Release();
            }
            //base.ChannelRead(context, message);
        }
    }
}
