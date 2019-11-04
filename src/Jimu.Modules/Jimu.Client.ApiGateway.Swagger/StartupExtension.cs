using Jimu.Client.ApiGateway.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Jimu.Client.ApiGateway
{
    public static class StartupExtension
    {
        public static Microsoft.AspNetCore.Builder.IApplicationBuilder UseJimuSwagger(this Microsoft.AspNetCore.Builder.IApplicationBuilder app, JimuSwaggerOptions options)
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{options.Version}/swagger.json", $"{options.Title} {options.Version}");
            });
            app.UseSwagger();
            return app;
        }
        public static IServiceCollection AddJimuSwagger(this IServiceCollection services, JimuSwaggerOptions options)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<JimuSwaggerDocumentFilter>();
                c.SwaggerDoc(options.Version, new OpenApiInfo { Title = options.Title, Version = options.Version });
            });
            return services;
        }

    }
}
