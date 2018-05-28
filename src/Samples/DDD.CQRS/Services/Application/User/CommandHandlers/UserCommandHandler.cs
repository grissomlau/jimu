using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices.Commands;
using MassTransit;

namespace DDD.CQRS.Services.Application.User.CommandHandlers
{
    public class UserCommandHandler :
        IConsumer<CreateUser>,
        IConsumer<UpdateUser>
    {
        public Task Consume(ConsumeContext<CreateUser> context)
        {
            Console.WriteLine("consume createuser");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<UpdateUser> context)
        {
            Console.WriteLine("consume updateuser");
            return Task.CompletedTask;
        }
    }
}
