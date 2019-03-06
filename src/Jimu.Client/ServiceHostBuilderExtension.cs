using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseLog4netLogger(this IServiceHostBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseLog4netLogger<IServiceHostClientBuilder>(options);
        }
        public static IServiceHostClientBuilder UseNLogger(this IServiceHostBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseNLogger<IServiceHostClientBuilder>(options);
        }
    }
}
