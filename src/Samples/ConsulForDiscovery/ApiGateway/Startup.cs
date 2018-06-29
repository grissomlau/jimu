using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Jimu;
using Jimu.Client;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiGateway
{
    /// <summary>
    /// inject controller sample
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
            services.UseJimu();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // start jimu client host;
            var host = new ServiceHostClientBuilder(new ContainerBuilder())
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-")
                .UseDotNettyForTransfer()
                .UseHttpForTransfer()
                .UsePollingAddressSelector()
                .Build();
            app.UseJimu(host);
            host.Run();

        }
    }
}
