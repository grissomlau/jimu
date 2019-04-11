using System;
using Auth.IService.DTO;
using Jimu;
using Jimu.Server.Auth;

namespace Auth.IService
{
    [JimuServiceRoute("/api/{Service}")]
    public interface IAuthService : IJimuService
    {
        [JimuService()]
        void Check(JwtAuthorizationContext context);
    }
}
