using Autofac;
using Jimu.Logger;

namespace Jimu
{
    public static partial class ServiceHostBuilderExtension
    {
        public static T UseLog4netLogger<T>(this IServiceHostBuilder serviceHostBuilder, LogOptions options = null)
            where T : class, IServiceHostBuilder
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                if (options == null) options = new LogOptions();
                containerBuilder.RegisterType<Log4netLogger>().WithParameter("options", options).As<ILogger>().SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use Log4net logger");
            });
            return serviceHostBuilder as T;
        }
        public static T UseNLogger<T>(this IServiceHostBuilder serviceHostBuilder, LogOptions options = null)
            where T : class, IServiceHostBuilder
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                if (options == null) options = new LogOptions();
                containerBuilder.RegisterType<NLogger>().WithParameter("options", options).As<ILogger>().SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use NLog logger");
            });
            return serviceHostBuilder as T;
        }
    }
}