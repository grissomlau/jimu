using Jimu;
using System.Collections.Generic;
using System.Threading.Tasks;
using IService.User.dto;

namespace IService.User
{
    /// <summary>
    /// try async 
    /// </summary>
    [Jimu("/{Service}")]
    public interface ITaskService : IJimuService
    {
        /// <summary>
        /// get void
        /// </summary>
        /// <returns></returns>
        [JimuGet(true)]
        Task GetVoidAsync();
        /// <summary>
        /// get user by id 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>user</returns>
        [JimuGet(true)]
        Task<UserModel> GetObjectAsync(int id);
        /// <summary>
        /// get all 
        /// </summary>
        /// <returns>user list</returns>
        [JimuGet(true)]
        Task<List<UserModel>> GetListAsync();

        /// <summary>
        /// get a string
        /// </summary>
        /// <param name="anything"></param>
        /// <returns></returns>
        [JimuGet(true)]
        Task<string> GetStringAsync(string anything);
    }
}
