using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jimu.Server
{
    public class Startup : IStartup
    {
        readonly Stack<Func<RequestDel, RequestDel>> _middlewares;
        private readonly IServiceEntryContainer _serviceEntryContainer;
        private readonly ILogger _logger;
        private readonly ISerializer _serializer;
        private readonly ITypeConvertProvider _typeConvert;
        public Startup(IConfiguration configuration, Stack<Func<RequestDel, RequestDel>> middlewares, IServiceEntryContainer serviceEntryContainer, ILogger logger, ISerializer serializer, ITypeConvertProvider typeConvert)
        {
            Configuration = configuration;
            _middlewares = middlewares;
            _serviceEntryContainer = serviceEntryContainer;
            _logger = logger;
            _serializer = serializer;
            _typeConvert = typeConvert;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseMiddleware<HttpMiddleware>(_middlewares, _serviceEntryContainer, _logger, _serializer, _typeConvert);
        }

    }
}
