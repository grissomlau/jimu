using Microsoft.AspNetCore.Http;

namespace Jimu.Client.ApiGateway.Core
{
    public static class JimuHttpContext
    {
        private static IHttpContextAccessor _accessor;

        public static HttpContext Current => _accessor.HttpContext;

        internal static void Configure(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
    }
}
