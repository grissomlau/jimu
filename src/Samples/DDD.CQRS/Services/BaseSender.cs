using Jimu.Server;
using MassTransit;

namespace DDD.CQRS.Services
{
    public abstract class BaseSender
    {
        protected readonly IBus Bus;
        protected readonly MassTransitOptions MassTransitOptions;

        protected BaseSender(IBus bus, MassTransitOptions options)
        {
            Bus = bus;
            MassTransitOptions = options;
        }
        protected virtual ISendEndpoint GetSender()
        {
            return Bus.GetSendEndpoint(MassTransitOptions.SendEndPointUri).Result;
        }
    }
}
