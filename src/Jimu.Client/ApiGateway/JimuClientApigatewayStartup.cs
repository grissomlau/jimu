using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway
{
    public abstract class JimuClientApiGatewayStartup: IJimuClientApiGatewayStartup
    {
        public virtual void ConfigureWebHost(IWebHostBuilder webBuilder)
        {

        }
        public virtual void ConfigureServices(IServiceCollection services)
        {

        }
        public virtual void Configure(IApplicationBuilder app)
        {

        }
    }
}
