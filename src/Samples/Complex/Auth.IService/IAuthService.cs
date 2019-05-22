using System;
using Auth.IService.DTO;
using Jimu;
using Jimu.Server.Auth;

namespace Auth.IService
{
    /// <summary>
    /// 鉴权
    /// </summary>
    [JimuServiceRoute("/api/{Service}")]
    public interface IAuthService : IJimuService
    {
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="context"></param>
        [JimuService()]
        void Check(JwtAuthorizationContext context);
    }
}
