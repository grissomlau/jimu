using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SkyApm;

namespace Jimu.Client.ApiGateway.Skywalking
{
    public class SkywalkingStartup : JimuClientApiGatewayStartup
    {

        public override void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ITracingDiagnosticProcessor, JimuClientDiagnosticProcessor>();
        }

        public override void ConfigureWebHost(IWebHostBuilder webBuilder)
        {
            webBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "SkyAPM.Agent.AspNetCore");
        }
    }
}
