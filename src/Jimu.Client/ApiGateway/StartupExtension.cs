using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

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
        public static IServiceCollection UseJimu(this IServiceCollection services)
        {
            //services.AddCors();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(o =>
            {
                o.ModelBinderProviders.Insert(0, new JimuQueryStringModelBinderProvider());
                o.ModelBinderProviders.Insert(1, new JimuModelBinderProvider());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            return services;
        }

    }
}
