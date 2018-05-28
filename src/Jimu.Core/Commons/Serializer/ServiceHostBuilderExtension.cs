using Autofac;
using Jimu.Core;
using Jimu.Core.Commons.Serializer;

namespace Jimu
{
    public static partial class ServiceHostBuilderExtension
    {
        internal static IServiceHostBuilder UseSerializer(this IServiceHostBuilder serviceHostBuilder)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<Serializer>().As<ISerializer>().SingleInstance();
            });
            return serviceHostBuilder;
        }
    }
}