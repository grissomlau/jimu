using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices.Commands;
using Jimu.Core.Protocols.Attributes;

namespace DDD.CQRS.IServices
{
    [ServiceRoute("api/{Service}")]
    public interface IUserService
    {
        [Service(CreatedBy = "grissom", Comment = "getuser")]
        string GetUser();
        [Service(CreatedBy = "grissom", Comment = "create user")]
        Task<Guid> CreateUser(CreateUser createUser);
    }
}
