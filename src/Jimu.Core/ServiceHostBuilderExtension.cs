using Jimu.Commons.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Jimu.MService
{
    public static partial class ServiceHostBuilderExtension
    {
        internal static IServiceHostBuilder UseLog4netLogger(this IServiceHostBuilder serviceHostBuilder, Log4netOptions options = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                if (options == null) options = new Log4netOptions();
                containerBuilder.RegisterType<Log4netLogger>().WithParameter("options", options).As<ILogger>().SingleInstance();
            });
            return serviceHostBuilder;
        }

        internal static IServiceHostBuilder UseSerializer(this IServiceHostBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService((containerBuilder) =>
            {
                containerBuilder.RegisterType<Serializer>().As<ISerializer>().SingleInstance();
            });
            return serviceHostBuilder;
        }
    }
}
