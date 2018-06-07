using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Jimu.Client
{
    public class DotNettyClientSender : IClientSender, IDisposable
    {
        private readonly IChannel _channel;
        private readonly ISerializer _serializer;
        public DotNettyClientSender(IChannel channel, ISerializer serializer)
        {
            _channel = channel;
            _serializer = serializer;
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await _channel.DisconnectAsync();
            }).Wait();
        }

        public async Task SendAsync(JimuTransportMsg message)
        {
            var buffer = GetByteBuffer(message);
            await _channel.WriteAndFlushAsync(buffer);
        }

        private IByteBuffer GetByteBuffer(JimuTransportMsg message)
        {
            var data = _serializer.Serialize<byte[]>(message);
            var buffer = Unpooled.Buffer(data.Length, data.Length);
            return buffer.WriteBytes(data);
        }

    }
}
