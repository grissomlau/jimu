using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Logger;

namespace Jimu.Server.Transport.DotNetty
{
    class ReadServerMessageChannelHandlerAdapter : ChannelHandlerAdapter
    {
        private readonly ILogger _logger;
        public ReadServerMessageChannelHandlerAdapter(ILogger logger)
        {
            _logger = logger;
        }
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var buffer = (IByteBuffer)message;
            try
            {
                byte[] data = new byte[buffer.ReadableBytes];
                buffer.GetBytes(buffer.ReaderIndex, data);

                _logger.Debug($"recevied msg is: {Encoding.UTF8.GetString(data)}");
                var convertedMsg = JimuHelper.Deserialize<byte[], JimuTransportMsg>(data);
                if (convertedMsg.ContentType == typeof(JimuRemoteCallData).FullName)
                {
                    convertedMsg.Content = JimuHelper.Deserialize<string, JimuRemoteCallData>(convertedMsg.Content.ToString());
                }
                else if (convertedMsg.ContentType == typeof(JimuRemoteCallResultData).FullName)
                {
                    convertedMsg.Content = JimuHelper.Deserialize<string, JimuRemoteCallResultData>(convertedMsg.Content.ToString());
                }
                context.FireChannelRead(convertedMsg);
                //context.FireChannelRead(data);
            }
            finally
            {
                buffer.Release();
            }
            //base.ChannelRead(context, message);
        }
    }
}
