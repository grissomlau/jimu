using Jimu;
using IService.User.dto;

namespace IService.User
{
    /// <summary>
    /// try Restful Api
    /// </summary>
    [Jimu("/{Service}")]
    public interface IRestService : IJimuService
    {
        /// <summary>
        /// get and return id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [JimuGet(true, Rest = "/{id}?name={name}")]
        string Get(int id, string name);
        //[JimuGet(true, Rest = "/{id}")]
        // Get(int id);
        /// <summary>
        /// get and return name and email
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>

        [JimuGet(true, Rest = "/{name}/{email}")]
        string Get(string name, string email);

        /// <summary>
        /// post user object and return name
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [JimuPost(true, Rest = "/")]
        string Post(UserModel user);

        /// <summary>
        /// delete by id and return deleted id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [JimuDelete(true, Rest = "/{id}")]
        int Delete(int id);

        [JimuGet(true, Rest = "/{id}/address")]
        string Address(int id);
    }
}
