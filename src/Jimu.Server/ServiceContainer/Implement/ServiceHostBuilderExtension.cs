using System.Collections.Generic;
using System.IO;
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
        ///     note: only one LoadServices invoke is affect, so don't double invoke LoadServices
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <param name="path">the path to load (trim the ".dll"), like: "services"</param>
        /// <param name="enableWatchChanged">reload when the path dll is updated</param>
        ///<param name="watchingFilePattern">what's the type of file will be watch when enableWatchChanged is true,e.g.: *.dll|*.json|*.config, default is *.dll</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder LoadServices(this IServiceHostServerBuilder serviceHostBuilder, string path, string watchingFilePattern = "*.dll", bool enableWatchChanged = true)
        {

            //var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes)
            //    .Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).ToList();


            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceEntryContainer>().As<IServiceEntryContainer>().SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var serviceEntryContainer = container.Resolve<IServiceEntryContainer>();
                var logger = container.Resolve<ILogger>();
                ServicesLoader servicesLoader = new ServicesLoader(serviceEntryContainer, logger, path, watchingFilePattern, enableWatchChanged);
                servicesLoader.LoadServices();
            });
            return serviceHostBuilder;
        }

        /// <summary>
        ///     load service(which must inherit from IServiceKey, and load Module of autofac
        ///     note: only one LoadServices invoke is affect, so don't double invoke LoadServices
        /// </summary>
        /// <param name="serviceHostBuilder"></param>
        /// <param name="assemblyNames">the dll to load (trim the ".dll"), like: "IServices,Services"</param>
        /// <returns></returns>
        public static IServiceHostServerBuilder LoadServices(this IServiceHostServerBuilder serviceHostBuilder, string[] assemblyNames)
        {

            //var serviceTypes = assemblies.SelectMany(x => x.ExportedTypes)
            //    .Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).ToList();


            serviceHostBuilder.RegisterService(containerBuilder =>
            {
                containerBuilder.RegisterType<ServiceEntryContainer>().As<IServiceEntryContainer>().SingleInstance();
            });
            serviceHostBuilder.AddInitializer(container =>
            {
                var serviceEntryContainer = container.Resolve<IServiceEntryContainer>();
                var logger = container.Resolve<ILogger>();
                var assemblies = new List<Assembly>();
                foreach (var assemblyName in assemblyNames)
                {
                    var name = assemblyName;
                    if (name.EndsWith(".dll")) name = name.Substring(0, name.Length - 4);
                    var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(name));
                    assemblies.Add(assembly);
                }

                serviceEntryContainer.LoadServices(assemblies);
            });
            return serviceHostBuilder;
        }
    }
}