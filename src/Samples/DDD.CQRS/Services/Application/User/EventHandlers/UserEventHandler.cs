using System;
using System.Threading.Tasks;
using DDD.CQRS.Domain.Events;
using MassTransit;

namespace DDD.CQRS.Services.Application.User.EventHandlers
{
    public class UserEventHandler : IConsumer<UserCreated>
    {
        public Task Consume(ConsumeContext<UserCreated> context)
        {
            Console.WriteLine("consume usercreated");
            return Task.CompletedTask;
        }
    }
}
