using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;

namespace Jimu.Server
{
    public static partial class ServiceHostBuilderExtension
    {
        /// <summary>
        ///     load service(which must inherit from IServiceKey, and load Module of autofac
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <param name="assemblyNames">the dll to load (trim the ".dll"), like: "IServices,Services"</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder LoadServices(this IServiceHostServerBuilder serviceHostBuilder,
            string[] assemblyNames)
        {
            var assemblies = new List<Assembly>();
            foreach (var assemblyName in assemblyNames)
            {
                var name = assemblyName;
                if (name.EndsWith(".dll")) name = name.Substring(0, name.Length - 4);
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name));
                assemblies.Add(assembly);
            }

            var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes)
            .Where(x => typeof(IJimuService).GetTypeInfo().IsAssignableFrom(x)).ToList();

            //var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes)
            //    .Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).ToList();


            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterTypes(serviceTypes.ToArray()).AsImplementedInterfaces()
                    .InstancePerDependency();
                containerBuilder.RegisterType<ServiceEntryContainer>().As<IServiceEntryContainer>().SingleInstance();
                containerBuilder.RegisterType<ServiceIdGenerator>().As<IServiceIdGenerator>().SingleInstance();
                // register module
                assemblies.ForEach(x => { containerBuilder.RegisterAssemblyModules(x); });
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var serviceEntryContainer = container.Resolve<IServiceEntryContainer>();
                serviceEntryContainer.AddServices(serviceTypes.ToArray());
            });
            return serviceHostBuilder;
        }
    }
}