using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Autofac;
using Jimu.Logger;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;

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
        private static bool IsCandidateLibrary(RuntimeLibrary library, AssemblyName assemblyName)
        {
            return (library.Name == (assemblyName.Name))
                    || (library.Dependencies.Any(d => d.Name.StartsWith(assemblyName.Name)));
        }
        private void LoadModule()
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
                Console.WriteLine($"【Error】load {name.Name} failed.");
                return null;
                //return context.LoadFromAssemblyName(name);
            };

#if DEBUG

            var debugReferencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            debugReferencedPaths.ToList().ForEach(path =>
            {
                AssemblyDependencyResolver(path);
            });

#endif


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


        private void AssemblyDependencyResolver(string path)
        {
            //AssemblyLoadContext.Default.LoadFromAssemblyName(assembly);
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            var dependencyContext = DependencyContext.Load(assembly);

            var assemblyResolver = new CompositeCompilationAssemblyResolver
                                     (new ICompilationAssemblyResolver[]
             {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
             });
            var loadContext = AssemblyLoadContext.GetLoadContext(assembly);

            dependencyContext?.RuntimeLibraries.ToList().ForEach(x =>
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
                var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

                foreach (var compilationLibrary in dependencyContext.CompileLibraries.Where(y => y.Name.StartsWith("Jimu")))
                {
                    if (!loadedPaths.Select(z => Path.GetFileNameWithoutExtension(z)).ToList().Contains(compilationLibrary.Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine($"dynamic load {compilationLibrary.Name}");

                        RuntimeLibrary library = dependencyContext.RuntimeLibraries.FirstOrDefault(runtime => runtime.Name == compilationLibrary.Name);
                        var wrapper = new CompilationLibrary(
                            library.Type,
                            library.Name,
                            library.Version,
                            library.Hash,
                            library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                            library.Dependencies,
                            library.Serviceable);
                        var assemblies = new List<string>();
                        assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                        if (assemblies.Count > 0)
                        {
                            loadContext.LoadFromAssemblyPath(assemblies[0]);
                        }
                    }
                    //DependencyDLL[library.Name] = cb;
                }
            });
        }
    }

}