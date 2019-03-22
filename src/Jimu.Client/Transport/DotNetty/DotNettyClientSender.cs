using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Jimu.Logger;

namespace Jimu.Client
{
    public class DotNettyClientSender : ClientSender, IDisposable
    {
        private readonly IChannel _channel;
        public DotNettyClientSender(ClientListener listener, ILogger logger, IChannel channel) : base(listener, logger)
        {
            _channel = channel;
        }

        public override void Dispose()
        {
            Task.Run(async () =>
            {
                await _channel.DisconnectAsync();
            }).Wait();

            base.Dispose();

        }

        protected override async Task DoSendAsync(JimuTransportMsg message)
        {
            var buffer = GetByteBuffer(message);
            await _channel.WriteAndFlushAsync(buffer);
        }

        private IByteBuffer GetByteBuffer(JimuTransportMsg message)
        {
            var data = JimuHelper.Serialize<byte[]>(message);
            var buffer = Unpooled.Buffer(data.Length, data.Length);
            return buffer.WriteBytes(data);
        }

    }
}
