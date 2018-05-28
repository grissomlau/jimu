using Autofac;
using MassTransit;

namespace DDD.CQRS.Services.Application
{
    public class HandlerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterConsumers(GetType().Assembly);
        }
    }
}
