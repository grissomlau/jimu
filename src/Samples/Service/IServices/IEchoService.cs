using System.Collections.Generic;
using System.Threading.Tasks;
using Jimu;

namespace IServices
{
    [JimuServiceRoute("api/echo")]
    public interface IEchoService : IJimuService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anything">任何值")]</param>
        /// <returns>返回anything"""\"</returns>
        [JimuService(CreatedBy = "grissom", Comment = "get input things", EnableAuthorization = false)]
        //[JimuFieldComment("anything", "任何值")]
        //[JimuReturnComment("返回anything")]
        string GetEchoAnonymous(string anything);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anything">任何值")]</param>
        /// <returns>返回anything"""\"</returns>
        [JimuService(CreatedBy = "grissom", Comment = "get input things", EnableAuthorization = false)]
        //[JimuFieldComment("anything", "任何值")]
        //[JimuReturnComment("返回anything")]
        UserDTO GetEchoAnonymous2(string anything);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="anything">任何值")]</param>
        /// <returns>返回anything"""\"</returns>
        [JimuService(CreatedBy = "grissom", Comment = "get input things", EnableAuthorization = true)]
        //[JimuFieldComment("anything", "任何值")]
        //[JimuReturnComment("返回anything")]
        string GetEcho(string anything);

        /// <summary>
        /// set echo and return the echo with indicate whether success flag"
        /// </summary>
        /// <param name="anything">任何值</param>
        /// <param name="anything2">任何值2</param>
        /// <returns></returns>
        //[JimuService(CreatedBy = "grissom", EnableAuthorization = true, Comment = "set echo and return the echo with indicate whether success flag")]
        [JimuService(CreatedBy = "grissom", EnableAuthorization = true)]
        //[JimuFieldComment("anything", "任何值")]
        //[JimuFieldComment("anything2", "任何值2")]
        //[JimuReturnComment("返回是否成功")]
        //Task<List<UserDTO>> SetEcho(string anything, string anything2);
        //[JimuService(EnableAuthorization = true)]
        Task<string> SetEcho(string anything);
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns></returns>
        [JimuService(CreatedBy = "grissom", Comment = "create user", EnableAuthorization = false, HttpMethod = "GET")]
        //[JimuFieldComment("user", "想创建的用户对象")]
        string CreateUser(UserDTO user);
        [JimuService(CreatedBy = "grissom", Comment = "create user friend")]
        string CreateUserFriend(UserDTO user, UserFriendDTO friend, string comment);

        [JimuService()]
        void CheckUser(Jimu.Server.Auth.JwtAuthorizationContext context);

        [JimuService()]
        List<UserDTO> GetTest(UserDTO userDTO);

   


    }
}
