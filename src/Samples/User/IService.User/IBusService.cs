using IService.User.dto;
using Jimu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IService.User
{
    /// <summary>
    /// message queue
    /// </summary>
    [Jimu]
    public interface IBusService : IJimuService
    {
        [JimuGet(true)]
        Task Send(UserReq req);

        [JimuGet(true)]
        Task<HelloResponse> Request(string greeting);
    }
}
