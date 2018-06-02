using System;
using System.Threading.Tasks;
using DDD.CQRS.IServices.Commands;
using Jimu;

namespace DDD.CQRS.IServices
{
    [JimuServiceRoute("api/{Service}")]
    public interface IUserService
    {
        [JimuServiceAttribute(CreatedBy = "grissom", Comment = "getuser")]
        string GetUser();
        [JimuServiceAttribute(CreatedBy = "grissom", Comment = "create user")]
        Task<Guid> CreateUser(CreateUser createUser);
    }
}
