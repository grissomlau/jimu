using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Jimu.Client.Proxy.CodeAnalysisIntegration;
using Jimu.Core.Client;
using Jimu.Core.Client.Proxy;
using Jimu.Core.Client.RemoteInvoker;
using Jimu.Core.Protocols;
using Jimu.Core.Server.ServiceContainer;

namespace Jimu
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostClientBuilder UseServiceProxy(this IServiceHostClientBuilder serviceHostBuilder, string[] assemblyNames)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceProxyGenerator>().As<IServiceProxyGenerator>().SingleInstance();
                containerBuilder.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
                containerBuilder.RegisterType<RemoteServiceInvoker>().As<IRemoteServiceInvoker>().SingleInstance();
                containerBuilder.RegisterType<ServiceIdGenerator>().As<IServiceIdGenerator>().SingleInstance();
                containerBuilder.RegisterType<TypeConvertProvider>().As<ITypeConvertProvider>().SingleInstance();
            });

            var assemblies = new List<Assembly>();
            foreach (var assemblyName in assemblyNames)
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                assemblies.Add(assembly);
            }

            var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes).Where(x => typeof(IService).GetTypeInfo().IsAssignableFrom(x) && x.IsInterface).ToList();

            serviceHostBuilder.AddInitializer(componentRegister =>
            {
                var serviceProxyGenerator = componentRegister.Resolve<IServiceProxyGenerator>();
                var serviceProxyTypes = serviceProxyGenerator.GenerateProxy(serviceTypes);
                var serviceProxy = componentRegister.Resolve<IServiceProxy>();
            });

            return serviceHostBuilder;
        }
    }
}
