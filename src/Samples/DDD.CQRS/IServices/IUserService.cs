using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices.Commands;
using Jimu.Core.Protocols.Attributes;

namespace DDD.CQRS.IServices
{
    [ServiceRoute("api/{Service}")]
    public interface IUserService
    {
        [Service(Director = "grissom", Name = "getuser")]
        string GetUser();
        [Service(Director = "grissom", Name = "create user")]
        Task<Guid> CreateUser(CreateUser createUser);
    }
}
