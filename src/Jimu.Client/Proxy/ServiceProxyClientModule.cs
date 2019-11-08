using Autofac;
using Jimu.Client.Proxy.Implement;
using Jimu.Logger;
using Jimu.Module;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jimu.Client.Proxy
{
    public class ServiceProxyClientModule : ClientModuleBase
    {
        private readonly ServiceProxyOptions _options;
        public ServiceProxyClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(ServiceProxyOptions).Name).Get<ServiceProxyOptions>();
        }

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            if (_options != null && _options.AssemblyNames != null && _options.AssemblyNames.Any())
            {
                componentContainerBuilder.RegisterType<ServiceProxyGenerator>().As<IServiceProxyGenerator>().SingleInstance();
                componentContainerBuilder.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
            }
            base.DoRegister(componentContainerBuilder);
        }

        public override void DoInit(IContainer container)
        {
            if (_options != null && _options.AssemblyNames != null && _options.AssemblyNames.Any())
            {
                var assemblies = new List<Assembly>();
                foreach (var assemblyName in _options.AssemblyNames)
                {
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
                    assemblies.Add(assembly);
                }

                var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes).Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).ToList();


                var serviceProxyGenerator = container.Resolve<IServiceProxyGenerator>();
                var serviceProxyTypes = serviceProxyGenerator.GenerateProxy(serviceTypes);
                var serviceProxy = container.Resolve<IServiceProxy>();
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use service proxy");
            }
            base.DoInit(container);
        }

    }
}
