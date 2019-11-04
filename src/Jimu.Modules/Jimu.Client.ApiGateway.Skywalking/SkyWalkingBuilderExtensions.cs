using Microsoft.Extensions.DependencyInjection;
using SkyApm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.ApiGateway.Skywalking
{
    public static class SkyWalkingBuilderExtensions
    {
        public static IServiceCollection AddJimuSkywalking (this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddSingleton<ITracingDiagnosticProcessor, JimuClientDiagnosticProcessor>();

            return services;
        }
    }
}
