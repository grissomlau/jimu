using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Jimu.Logger;

namespace Jimu.Server
{
    public class ApplicationServerBuilder : ApplicationBuilderBase<ApplicationServerBuilder>
    {

        public List<Action<ContainerBuilder>> ServiceRegisters = new List<Action<ContainerBuilder>>();
        public List<Action<IContainer>> ServiceInitializers = new List<Action<IContainer>>();

        public ApplicationServerBuilder(ContainerBuilder containerBuilder, string settingName = "JimuAppServerSettings") : base(containerBuilder, settingName)
        {
        }

        public override ApplicationServerBuilder AddComponent(Action<ContainerBuilder> componentRegister)
        {
            return base.AddComponent(componentRegister);
        }

        public override ApplicationServerBuilder AddInitializer(Action<IContainer> initializer)
        {
            return base.AddInitializer(initializer);
        }

        public override ApplicationServerBuilder AddRunner(Action<IContainer> runner)
        {
            return base.AddRunner(runner);
        }

        public virtual ApplicationServerBuilder AddServiceComponent(Action<ContainerBuilder> serviceRegister)
        {
            ServiceRegisters.Add(serviceRegister);
            return this;
        }
        public virtual ApplicationServerBuilder AddServiceInitializer(Action<IContainer> initializer)
        {
            ServiceInitializers.Add(initializer);
            return this;
        }


        public override IApplication Build()
        {
            LoadComponent();
            this.AddInitializer(container =>
            {
                var serviceEntry = container.Resolve<IServiceEntryContainer>();
                this.ServiceRegisters.ForEach(x => serviceEntry.DoRegister(x));
                this.ServiceInitializers.ForEach(x => serviceEntry.DoInitializer(x));
            });

            return base.Build();
        }

        private void LoadComponent()
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(path))));


            var type = typeof(ServerComponentBase);
            var components = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract);
            components.ToList().ForEach(x =>
            {
                var comp = Activator.CreateInstance(x, this.JimuAppSettings) as ServerComponentBase;
                if (comp != null)
                {
                    this.AddInitializer(comp.DoInit);
                    this.AddRunner(comp.DoRun);
                    this.AddComponent(comp.DoRegister);
                    this.AddServiceComponent(comp.DoServiceRegister);
                    this.AddServiceInitializer(comp.DoServiceInit);
                    this.AddBeforeRunner(comp.DoBeforeRun);
                }
            });

        }
    }
}