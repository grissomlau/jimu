using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jimu.Client.ApiGateway
{
    public interface IJimuClientApiGatewayStartup
    {
        void ConfigureWebHost(IWebHostBuilder webBuilder);
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app);
    }
}
