using Jimu;
using System;
using System.Collections.Generic;
using System.Text;
using User.IService.dto;

namespace User.IService
{
    /// <summary>
    /// 试试 Restful Api
    /// </summary>
    [JimuServiceRoute("/{Service}")]
    public interface IRestService : IJimuService
    {
        /// <summary>
        /// get and return id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [JimuGet(true, Rest = "/{id}")]
        int Get(int id);
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
        [JimuPost(true)]
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
