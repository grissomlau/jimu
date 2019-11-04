using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;

namespace Jimu.Client.ApiGateway
{
    public static class StartupExtension
    {
        public static Microsoft.AspNetCore.Builder.IApplicationBuilder UseJimu(this Microsoft.AspNetCore.Builder.IApplicationBuilder app, IApplication host)
        {
            Console.WriteLine();
            app.UseMiddleware<JimuHttpStatusCodeExceptionMiddleware>();
            //app.UseCors(builder =>
            //{
            //    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            //});

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
        public static IServiceCollection AddJimu(this IServiceCollection services)
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

            return services;
        }

    }
}
