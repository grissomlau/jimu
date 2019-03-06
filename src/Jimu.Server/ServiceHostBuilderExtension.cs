using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseLog4netLogger(this IServiceHostServerBuilder serviceHostBuilder, LogOptions options = null)
        {

            return serviceHostBuilder.UseLog4netLogger<IServiceHostServerBuilder>(options);
        }
        public static IServiceHostServerBuilder UseNLogger(this IServiceHostServerBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseNLogger<IServiceHostServerBuilder>(options);
        }

    }
}
