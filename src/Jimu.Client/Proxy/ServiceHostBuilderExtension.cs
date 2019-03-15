using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Jimu.Client.Proxy;
using Jimu.Client.Proxy.CodeAnalysisIntegration;

namespace Jimu.Client
{
    public static partial class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseServiceProxy(this IServiceHostClientBuilder serviceHostBuilder, ServiceProxyOptions options)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceProxyGenerator>().As<IServiceProxyGenerator>().SingleInstance();
                containerBuilder.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
                containerBuilder.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().SingleInstance();
            });

            var assemblies = new List<Assembly>();
            foreach (var assemblyName in options.AssemblyNames)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                assemblies.Add(assembly);
            }

            var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).ToList();

            serviceHostBuilder.AddInitializer(componentRegister =>
            {
                var serviceProxyGenerator = componentRegister.Resolve<IServiceProxyGenerator>();
                var serviceProxyTypes = serviceProxyGenerator.GenerateProxy(serviceTypes);
                var serviceProxy = componentRegister.Resolve<IServiceProxy>();
                var logger = componentRegister.Resolve<ILogger>();
                logger.Info($"[config]use service proxy");
            });

            return serviceHostBuilder;
        }
    }
}
