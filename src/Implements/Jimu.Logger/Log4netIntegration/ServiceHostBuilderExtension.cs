using Autofac;
using Jimu.Common.Logger.Log4netIntegration;


namespace Jimu
{
    public static class ServiceHostBuilderExtension
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
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseLog4netLogger(this IServiceHostServerBuilder serviceHostBuilder, Log4netOptions options = null)
        {
            return serviceHostBuilder.UseLog4netLogger<IServiceHostServerBuilder>(options);
        }

    }
}
namespace Jimu.Client
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseLog4netLogger(this IServiceHostBuilder serviceHostBuilder, Log4netOptions options = null)
        {
            return serviceHostBuilder.UseLog4netLogger<IServiceHostClientBuilder>(options);
        }
    }
}
