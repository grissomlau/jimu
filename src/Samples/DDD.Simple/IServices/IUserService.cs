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
        [Service(Director = "grissom", Name = "getuser", EnableAuthorization = true)]
        UserDto GetUser(Guid id);
        [Service(Director = "grissom", Name = "create user")]
        Task<Guid> CreateUser(UserCreateReq userCreateReq);
        [Service(Director = "grissom", Name = "get user by name")]
        Task<UserDto> GetByName(string userName);

        [Service(Director = "grissom", Name = "change user name")]
        Task<Guid> UpdateUser(UserNameChangeReq userNameChangeReq);


        [Service(Director = "grissom", Name = "get file")]
        //Task<Byte[]> GetFile();
        Task<FileModel> GetFile();

        [Service(Director = "grissom", Name = "get file")]
        Task UploadFiles(List<FileModel> files);
    }
}
