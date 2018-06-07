using Autofac;
using Jimu.Client;
using Jimu.Common.Logger;

namespace Jimu
{
    public static partial class ServiceHostBuilderExtension
    {
        /// <summary>
        ///     use console lgo
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <returns></returns>
        internal static IServiceHostBuilder UseConsoleLogger(this IServiceHostBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ConsoleLogger>().As<ILogger>().SingleInstance();
            });
            return serviceHostBuilder;
        }
    }

    public static partial class ServiceHostBuilderExtension
    {
        public static T UseLog4netLogger<T>(this IServiceHostBuilder serviceHostBuilder, Log4netOptions options = null)
            where T : class, IServiceHostBuilder
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                if (options == null) options = new Log4netOptions();
                containerBuilder.RegisterType<Log4netLogger>().WithParameter("options", options).As<ILogger>().SingleInstance();
            });
            return serviceHostBuilder as T;
        }
    }
}
namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseLog4netLogger(this IServiceHostServerBuilder serviceHostBuilder, Log4netOptions options = null)
        {
            options = options ?? new Log4netOptions { EnableConsoleLog = true, LogLevel = LogLevel.Error | LogLevel.Info };
            return serviceHostBuilder.UseLog4netLogger<IServiceHostServerBuilder>(options);
        }

    }
}
namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseLog4netLogger(this IServiceHostBuilder serviceHostBuilder, Log4netOptions options = null)
        {
            return serviceHostBuilder.UseLog4netLogger<IServiceHostClientBuilder>(options);
        }
    }
}