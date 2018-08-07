using System.Collections.Generic;
using System.Threading.Tasks;
using Jimu;

namespace IServices
{
    [JimuServiceRoute("api/echo")]
    public interface IEchoService : IJimuService
    {
        [JimuService(CreatedBy = "grissom", Comment = "get input things", EnableAuthorization = true)]
        [JimuFieldComment("anything", "任何值")]
        [JimuReturnComment("返回anything")]
        string GetEcho(string anything);

        [JimuService(CreatedBy = "grissom", EnableAuthorization = true, Comment = "set echo and return the echo with indicate whether success flag")]
        [JimuFieldComment("anything", "任何值")]
        [JimuFieldComment("anything2", "任何值2")]
        [JimuReturnComment("返回是否成功")]
        Task<List<UserDTO>> SetEcho(string anything, string anything2);
        [JimuService(CreatedBy = "grissom", Comment = "create user")]
        [JimuFieldComment("user", "想创建的用户对象")]
        string CreateUser(UserDTO user);
        [JimuService(CreatedBy = "grissom", Comment = "create user friend")]
        string CreateUserFriend(UserDTO user, UserFriendDTO friend, string comment);
    }
}
