using Jimu.Bus;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitRabbitMqRequestContext<T> : IJimuRequestContext<T> where T : IJimuRequest
    {
        public MassTransitRabbitMqRequestContext(IJimuBus bus, T message)
        {
            JimuBus = bus;
            Message = message;
        }

        public T Message { get; private set; }

        public IJimuBus JimuBus { get; private set; }
    }
}
