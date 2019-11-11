using Autofac;
using Autofac.Extensions.DependencyInjection;
using Jimu.Client.ApiGateway.Core;
using Jimu.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;

namespace Jimu.Client.ApiGateway
{
    public static class WebHostBuilderExtension
    {
        internal static IHostBuilder UseWebHostJimu(this IHostBuilder hostBuilder, string settingName = "JimuAppClientSettings", Action<IServiceCollection> servicesAction = null, Action<WebHostBuilderContext, IApplicationBuilder> appAction = null, IApplication host = null)
        {
            if (host == null)
                host = new ApplicationClientBuilder(new ContainerBuilder(), settingName).Build();

            hostBuilder.ConfigureWebHostDefaults(web =>
            {
                web
                .UseKestrel()
                .UseIIS()
                .ConfigureServices(services =>
                {
                    services.AddControllers();
                    servicesAction?.Invoke(services);
                    services.AddJimu(host);
                })
                .Configure((context, app) =>
                {
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    app.UseRouting();
                    app.UseAuthorization();
                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapControllers();
                    });

                    appAction?.Invoke(context, app);
                    app.UseJimu(host);
                });
            });

            host.Run();
            JimuClient.Host = host;
            return hostBuilder;
        }

        public static IServiceCollection AddJimu(this IServiceCollection services, IApplication host)
        {
            //services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(o =>
            {
                o.EnableEndpointRouting = false;
                o.ModelBinderProviders.Insert(0, new JimuQueryStringModelBinderProvider());
                o.ModelBinderProviders.Insert(1, new JimuModelBinderProvider());
                o.RespectBrowserAcceptHeader = true;
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            })
            //.AddJsonOptions(options =>
            //{
            //    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            //})
            .AddXmlSerializerFormatters()
            ;

            var type = typeof(ClientWebModuleBase);
            var configures = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x, host.JimuAppSettings) as ClientWebModuleBase)
                .OrderBy(x => x.Priority);

            foreach (var configure in configures)
            {
                configure.DoWebConfigureServices(services, host.Container);
            }
            return services;
        }

        public static IApplicationBuilder UseJimu(this IApplicationBuilder app, IApplication host)
        {
            var type = typeof(ClientWebModuleBase);
            var configures = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x, host.JimuAppSettings) as ClientWebModuleBase)
                .OrderBy(x => x.Priority);

            app.UseMiddleware<JimuHttpStatusCodeExceptionMiddleware>();
            //app.UseCors(builder =>
            //{
            //    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            //});


            foreach (var configure in configures)
            {
                configure.DoWebConfigure(app, host.Container);
            }

            var httpContextAccessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            JimuHttpContext.Configure(httpContextAccessor);
            app.UseMvc(routes =>
            {

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}");
                routes.MapRoute(
                    name: "defaultApi",
                    template: "api/{controller}/{action}");
                routes.MapRoute(
                   "swagger",
                  "swagger/{*path}"
                  );
                routes.MapRoute(
                    "JimuPath",
                    "{*path:regex(^(?!swagger))}",
                    new { controller = "JimuServices", action = "JimuPath" });
            });

            JimuClient.Host = host;
            return app;
        }
    }
}
