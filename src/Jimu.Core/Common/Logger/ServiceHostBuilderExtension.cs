using Autofac;

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
}