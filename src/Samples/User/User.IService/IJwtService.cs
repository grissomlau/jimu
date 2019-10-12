using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace User.IService
{
    /// <summary>
    /// try jwt
    /// </summary>
    [JimuServiceRoute("/{Service}")]
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
