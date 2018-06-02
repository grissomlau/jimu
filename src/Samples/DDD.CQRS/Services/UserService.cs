using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices;
using DDD.CQRS.IServices.Commands;
using Jimu.Server;
using MassTransit;

namespace DDD.CQRS.Services
{
    public class UserService : BaseSender, IUserService
    {
        public UserService(IBus bus, MassTransitOptions options) : base(bus, options)
        {
        }
        public string GetUser()
        {
            return "grissom";
        }

        public async Task<Guid> CreateUser(CreateUser createUser)
        {
            createUser.Id = Guid.NewGuid();
            await GetSender().Send(createUser);
            return createUser.Id;
        }
    }
}
