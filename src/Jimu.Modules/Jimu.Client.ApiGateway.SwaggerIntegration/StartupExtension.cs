using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Client.ApiGateway.SwaggerIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;

namespace Jimu.Client.ApiGateway
{
    public static class StartupExtension
    {
        public static Microsoft.AspNetCore.Builder.IApplicationBuilder UseJimuSwagger(this Microsoft.AspNetCore.Builder.IApplicationBuilder app, string version = "v1")
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"Jimu API {version}");
            });
            app.UseSwagger();
            return app;
        }
        public static IServiceCollection UseJimuSwagger(this IServiceCollection services, JimuSwaggerOptions options)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<JimuSwaggerDocumentFilter>();
                c.SwaggerDoc(options.Version, new Info { Title = options.Title, Version = options.Version });
            });
            return services;
        }

    }
}
