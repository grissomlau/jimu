using Jimu.Bus;
using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitRabbitMqBus : IJimuBus
    {
        readonly IBus _bus;
        readonly MassTransitOptions _options;
        public MassTransitRabbitMqBus(IBus bus, MassTransitOptions options)
        {
            _bus = bus;
            _options = options;
        }
        public async Task PublishAsync<T>(T @event) where T : IJimuEvent
        {
            await _bus.Publish(@event);
        }

        public async Task<Resp> RequestAsync<Req, Resp>(Req request, TimeSpan timeout = default, CancellationToken cancellationToken = default)
            where Req : class, IJimuRequest
            where Resp : class
        {
            if (timeout == default)
                timeout = TimeSpan.FromSeconds(_options.RequestTimeoutSeconds);
            var client = _bus.CreateRequestClient<Req>(new Uri($"queue:{request.QueueName}"), timeout);
            //var client = _bus.CreateRequestClient<Req, Resp>(new Uri($"queue:{request.QueueName}"), timeout);
            //return client.GetResponse<Resp>(request, cancellationToken).GetAwaiter().GetResult().Message;
            var resp = await client.GetResponse<Resp>(request, cancellationToken);
            return resp.Message;
        }

        public async Task SendAsync<T>(T command) where T : IJimuCommand
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{command.QueueName}"));
            await endpoint.Send(command);
        }
    }
}
