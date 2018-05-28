using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Serializer;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration.TransportClient
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

        public async Task SendAsync(TransportMessage message)
        {
            var buffer = GetByteBuffer(message);
            await _channel.WriteAndFlushAsync(buffer);
        }

        private IByteBuffer GetByteBuffer(TransportMessage message)
        {
            var data = _serializer.Serialize<byte[]>(message);
            var buffer = Unpooled.Buffer(data.Length, data.Length);
            return buffer.WriteBytes(data);
        }

    }
}
