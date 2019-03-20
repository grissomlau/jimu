using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server
{
    public static partial class ApplicationBuilderExtension
    {
        public static IApplicationServerBuilder UseLog4netLogger(this IApplicationServerBuilder serviceHostBuilder, LogOptions options = null)
        {

            return serviceHostBuilder.UseLog4netLogger<IApplicationServerBuilder>(options);
        }
        public static IApplicationServerBuilder UseNLogger(this IApplicationServerBuilder serviceHostBuilder, LogOptions options = null)
        {
            return serviceHostBuilder.UseNLogger<IApplicationServerBuilder>(options);
        }

    }
}
