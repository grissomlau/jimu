using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jimu.Client.ApiGateway
{
    public static class WebHostBuilderExtension
    {
        public static IWebHostBuilder UseJimu(this IWebHostBuilder @this, string settingName = "JimuAppClientSettings")
        {
            var host = new ApplicationClientBuilder(new ContainerBuilder(), settingName).Build();

            var type = typeof(IJimuClientApiGatewayStartup);
            var startups = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x) as IJimuClientApiGatewayStartup);

            foreach (var su in startups)
            {
                su.ConfigureWebHost(@this);
            }

            @this.ConfigureServices(services =>
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

                foreach (var su in startups)
                {
                    su.ConfigureServices(services);
                }

            });

            @this.Configure(app =>
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

                foreach (var su in startups)
                {
                    su.Configure(app);
                }


                JimuClient.Host = host;
                host.Run();
            });

            return @this;
        }
    }
}
