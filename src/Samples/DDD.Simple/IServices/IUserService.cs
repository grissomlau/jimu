using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDD.Simple.IServices.DTO;
using Jimu.Core.Protocols;
using Jimu.Core.Protocols.Attributes;

namespace DDD.Simple.IServices
{
    [ServiceRoute("api/simple/{Service}")]
    public interface IUserService : IService
    {
        [Service(CreatedBy = "grissom", Comment = "getuser", EnableAuthorization = true)]
        UserDto GetUser(Guid id);
        [Service(CreatedBy = "grissom", Comment = "create user")]
        Task<Guid> CreateUser(UserCreateReq userCreateReq);
        [Service(CreatedBy = "grissom", Comment = "get user by name")]
        Task<UserDto> GetByName(string userName);

        [Service(CreatedBy = "grissom", Comment = "change user name")]
        Task<Guid> UpdateUser(UserNameChangeReq userNameChangeReq);


        [Service(CreatedBy = "grissom", Comment = "get file")]
        //Task<Byte[]> GetFile();
        Task<FileModel> GetFile();

        [Service(CreatedBy = "grissom", Comment = "get file")]
        Task UploadFiles(List<FileModel> files);
    }
}
