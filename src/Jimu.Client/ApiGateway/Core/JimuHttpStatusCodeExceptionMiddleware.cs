using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jimu.Client.ApiGateway
{
    public class JimuHttpStatusCodeExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public JimuHttpStatusCodeExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (JimuHttpStatusCodeException ex)
            {
                context.Response.Clear();
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = ex.ContentType;
                await context.Response.WriteAsync(ex.Message);
            }
        }
    }
}
