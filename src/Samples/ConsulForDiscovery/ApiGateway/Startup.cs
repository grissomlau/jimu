using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ApiGateway.Controllers;
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
        private readonly ContainerBuilder _containerBuilder;
        IServiceHost _host;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _containerBuilder = new ContainerBuilder();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddControllersAsServices();
            services.UseJimu();

            // inject controller
            this._containerBuilder.RegisterAssemblyTypes(typeof(ValuesController).GetTypeInfo().Assembly)
                .Where(t => typeof(Controller).IsAssignableFrom(t)
                            && t.Name.EndsWith("Controller", StringComparison.Ordinal)).PropertiesAutowired();

            _containerBuilder.Populate(services);

            // start jimu client host;
            _host = new ServiceHostClientBuilder(_containerBuilder)
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-")
                .UseDotNettyForTransfer()
                .UseHttpForTransfer()
                .UsePollingAddressSelector()
                .UseServiceProxy(new[] { "Simple.IServices" })
                .Build();

            return this._host.Container.Resolve<IServiceProvider>();
            //return new AutofacServiceProvider(this._host.Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseJimu(_host);
            _host.Run();

        }
    }
}
