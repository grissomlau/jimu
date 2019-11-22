using Jimu;
using Jimu.Server.Auth;
using Jimu.Server.Auth.Middlewares;

namespace IService.Auth
{
    /// <summary>
    /// 鉴权
    /// </summary>
    [Jimu("/Auth/{Service}")]
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
