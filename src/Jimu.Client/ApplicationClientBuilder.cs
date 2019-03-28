using System;
using Autofac;
using Jimu.Logger;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Jimu.Client
{
    public class ApplicationClientBuilder : ApplicationBuilderBase<ApplicationClientBuilder>
    {
        public ApplicationClientBuilder(ContainerBuilder containerBuilder, string settingName = "JimuAppClientSettings") : base(containerBuilder, settingName)
        {
            this.AddModule(cb =>
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


        public override IApplication Build()
        {
            LoadModule();
            return base.Build();
        }

        private void LoadModule()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedNames = loadedAssemblies.Select(a => a.GetName().Name).ToArray();
            //var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedNames.Contains(Path.GetFileNameWithoutExtension(r), StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));

            var type = typeof(ClientModuleBase);
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract);
            components.ToList().ForEach(x =>
            {
                var comp = Activator.CreateInstance(x, this.JimuAppSettings) as ClientModuleBase;
                if (comp != null)
                {
                    this.AddInitializer(comp.DoInit);
                    this.AddRunner(comp.DoRun);
                    this.AddModule(comp.DoRegister);
                }
            });

        }

    }
}