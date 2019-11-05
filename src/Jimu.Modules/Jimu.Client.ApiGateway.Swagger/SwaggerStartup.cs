using Jimu.Client.ApiGateway;
using Jimu.Client.ApiGateway.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Jimu.Client.ApiGateway
{
    internal class SwaggerStartup : JimuClientApiGatewayStartup
    {
        JimuSwaggerOptions _options;
        public SwaggerStartup()
        {
            _options = new JimuSwaggerOptions();
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{_options.Version}/swagger.json", $"{_options.Title} {_options.Version}");
            });
            app.UseSwagger();
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.DocumentFilter<JimuSwaggerDocumentFilter>();
                c.SwaggerDoc(_options.Version, new OpenApiInfo { Title = _options.Title, Version = _options.Version });
            });

        }
    }
}
