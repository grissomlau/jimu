using IService.User.dto;
using Jimu;
using System.Collections.Generic;

namespace IService.User
{
    /// <summary>
    /// hello world
    /// </summary>
    [Jimu("/{Service}")]
    public interface IHelloWorldService : IJimuService
    {
        /// <summary>
        /// get something
        /// </summary>
        /// <returns></returns>
        [JimuGet(true)]
        string Get();
        [JimuGet(true)]
        UserModel GetUserByObj(UserReq req);
        [JimuGet(true)]
        List<string> GetUserByArray(List<string> req);
        [JimuGet(true)]
        void LongTask(int count);
        [JimuGet(true)]
        void FastTask();
    }
}
