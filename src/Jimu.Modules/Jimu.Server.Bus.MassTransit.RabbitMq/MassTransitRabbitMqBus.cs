using Jimu.Core.Bus;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitRabbitMqBus : IJimuBus
    {
        readonly IBus _bus;
        public MassTransitRabbitMqBus(IBus bus)
        {
            _bus = bus;
        }
        public async Task PublishAsync<T>(T @event) where T : IJimuEvent
        {
            await _bus.Publish(@event);
        }

        public async Task SendAsync<T>(T command) where T : IJimuCommand
        {
            var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{command.QueueName}"));
            await endpoint.Send(command);
        }
    }
}
