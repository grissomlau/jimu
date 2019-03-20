using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IApplicationClientBuilder UseLog4netLogger(this IApplicationBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseLog4netLogger<IApplicationClientBuilder>(options);
        }
        public static IApplicationClientBuilder UseNLogger(this IApplicationBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseNLogger<IApplicationClientBuilder>(options);
        }
    }
}
