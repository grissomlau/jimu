using Autofac;
using Jimu.Client.Discovery;
using Jimu.Client.Discovery.Implement;
using Jimu.Client.Proxy;
using Jimu.Client.Proxy.Implement;
using Jimu.Client.RemoteCaller;
using Jimu.Client.RemoteCaller.Implement;
using Jimu.Client.Token;
using Jimu.Client.Token.Implement;
using Jimu.Client.Transport;
using Jimu.Module;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Jimu.Client
{
    public class ApplicationClientBuilder : ApplicationBuilderBase<ApplicationClientBuilder>
    {
        public ApplicationClientBuilder(ContainerBuilder containerBuilder, string settingName = "JimuAppClientSettings") : base(containerBuilder, settingName)
        {
            this.AddRegister(cb =>
            {
                cb.RegisterType<RemoteServiceCaller>().As<IRemoteServiceCaller>().SingleInstance();
                cb.RegisterType<ClientServiceDiscovery>().As<IClientServiceDiscovery>().SingleInstance();
                cb.RegisterType<ClientSenderFactory>().AsSelf().SingleInstance();
                cb.RegisterType<ServiceProxy>().As<IServiceProxy>().SingleInstance();
                cb.RegisterType<ServiceTokenGetter>().As<IServiceTokenGetter>().SingleInstance();
            });
            this.AddRunner(container =>
            {
                var clientServiceDiscovery = (ClientServiceDiscovery)container.Resolve<IClientServiceDiscovery>();
                clientServiceDiscovery?.RunInInit().Wait();

            });
        }


        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
        }

        protected override void LoadModule()
        {

            AssemblyLoadContext.Default.Resolving += (context, name) =>
            {
                // avoid loading *.resources dlls, because of: https://github.com/dotnet/coreclr/issues/8416
                if (name.Name.EndsWith("resources"))
                {
                    return null;
                }

                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (IsCandidateLibrary(library, name))
                    {
                        return context.LoadFromAssemblyName(new AssemblyName(library.Name));
                    }
                }

                var foundDlls = Directory.GetFileSystemEntries(new FileInfo(AppDomain.CurrentDomain.BaseDirectory).FullName, name.Name + ".dll", SearchOption.AllDirectories);
                if (foundDlls.Any())
                {
                    using (var sr = File.OpenRead(foundDlls[0]))
                    {
                        return context.LoadFromStream(sr);
                    }
                }

                //_logger.Warn($"cannot found assembly {name.Name}, path: { _options.Path}");
                return null;
                //return context.LoadFromAssemblyName(name);
            };

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedNames = loadedAssemblies.Select(a => a.GetName().Name).ToArray();
            //var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedNames.Contains(Path.GetFileNameWithoutExtension(r), StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            var type = typeof(ClientModuleBase);
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x, this.JimuAppSettings) as ClientModuleBase)
                .OrderBy(x => x.Priority);
            components.ToList().ForEach(x =>
            {
                this.AddInitializer(x.DoInit);
                this.AddRunner(x.DoRun);
                this.AddRegister(x.DoRegister);
            });

        }

    }
}