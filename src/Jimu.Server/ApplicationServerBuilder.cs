using Autofac;
using Jimu.Module;
using Jimu.Server.ServiceContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Jimu.Server
{
    public class ApplicationServerBuilder : ApplicationBuilderBase<ApplicationServerBuilder>
    {

        public List<Action<ContainerBuilder>> ServiceRegisters = new List<Action<ContainerBuilder>>();
        public List<Action<IContainer>> ServiceInitializers = new List<Action<IContainer>>();

        public ApplicationServerBuilder(ContainerBuilder containerBuilder, string settingName = "JimuAppServerSettings") : base(containerBuilder, settingName)
        {
        }

        public override ApplicationServerBuilder AddModule(Action<ContainerBuilder> moduleRegister)
        {
            return base.AddModule(moduleRegister);
        }

        public override ApplicationServerBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer);
        }

        public override ApplicationServerBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner);
        }

        public virtual ApplicationServerBuilder AddServiceModule(Action<ContainerBuilder> moduleRegister)
        {
            ServiceRegisters.Add(moduleRegister);
            return this;
        }
        public virtual ApplicationServerBuilder AddServiceInitializer(Action<IContainer> initializer)
        {
            ServiceInitializers.Add(initializer);
            return this;
        }


        public override IApplication Build()
        {
            LoadModule();
            this.AddInitializer(container =>
            {
                var serviceEntry = container.Resolve<IServiceEntryContainer>();
                this.ServiceRegisters.ForEach(x => serviceEntry.DoRegister(x));
                this.ServiceInitializers.ForEach(x => serviceEntry.DoInitializer(x));
            });

            return base.Build();
        }
        private void LoadModule()
        {

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));


            var type = typeof(ServerModuleBase);
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x, this.JimuAppSettings) as ServerModuleBase)
                .OrderBy(x => x.Priority); ;
            components.ToList().ForEach(x =>
                {
                    this.AddInitializer(x.DoInit);
                    this.AddRunner(x.DoRun);
                    this.AddModule(x.DoRegister);
                    this.AddServiceModule(x.DoServiceRegister);
                    this.AddServiceInitializer(x.DoServiceInit);
                    this.AddBeforeRunner(x.DoBeforeRun);

                });

        }
    }

}