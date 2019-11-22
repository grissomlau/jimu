using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace IService.User
{
    /// <summary>
    /// try jwt
    /// </summary>
    [Jimu("/{Service}")]
    public interface IJwtService : IJimuService
    {
        /// <summary>
        /// 需要 token
        /// </summary>
        /// <returns></returns>
        [JimuGet]
        string Get();
    }
}
