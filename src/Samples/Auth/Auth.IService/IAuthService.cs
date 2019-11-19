using Jimu;
using Jimu.Server.Auth;
using Jimu.Server.Auth.Middlewares;

namespace Auth.IService
{
    /// <summary>
    /// 鉴权
    /// </summary>
    [JimuServiceRoute("/Auth/{Service}")]
    public interface IAuthService : IJimuService
    {

        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="context"></param>
        [JimuService()]
        void Check(JwtAuthorizationContext context);
        /// <summary>
        /// 获取当前用户名称
        /// </summary>
        /// <returns></returns>
        [JimuGet()]
        string GetCurrentUserName();
    }
}
