using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway
{
    public interface IJimuClientApiGatewayStartup
    {
        void ConfigureWebHost(IWebHostBuilder webBuilder);
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app);
    }
}
