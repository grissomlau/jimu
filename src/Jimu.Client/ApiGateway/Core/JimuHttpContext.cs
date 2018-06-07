using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Jimu.Client.ApiGateway
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

    //public static class StaticHttpContextExtensions
    //{
    //    public static void AddHttpContextAccessor(this IServiceCollection services)
    //    {
    //        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    //    }

    //    public static IApplicationBuilder UseStaticHttpContext(this IApplicationBuilder app)
    //    {
    //        var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
    //        JimuHttpContext.Configure(httpContextAccessor);
    //        return app;
    //    }
    //}
}
