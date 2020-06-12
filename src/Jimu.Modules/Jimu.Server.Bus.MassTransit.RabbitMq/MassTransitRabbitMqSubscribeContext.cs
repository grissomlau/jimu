using Jimu.Core.Bus;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitRabbitMqSubscribeContext<T> : IJimuSubscribeContext<T> where T : IJimuEvent
    {
        public MassTransitRabbitMqSubscribeContext(IJimuBus bus, T message)
        {
            JimuBus = bus;
            Message = message;
        }

        public T Message { get; private set; }

        public IJimuBus JimuBus { get; private set; }
    }
}
