using Autofac;
using Autofac.Core;
using Jimu.Server.Implement.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jimu.Server
{
    public class ServiceEntryContainer : IServiceEntryContainer
    {
        private readonly List<JimuServiceEntry> _services = new List<JimuServiceEntry>();

        List<Action<ContainerBuilder>> _serviceRegisters;
        List<Action<IContainer>> _serviceInitializers;

        public event Action<List<JimuServiceEntry>> OnServiceLoaded;

        public ServiceEntryContainer()
        {
            this._serviceRegisters = new List<Action<ContainerBuilder>>();
            this._serviceInitializers = new List<Action<IContainer>>();
        }
        /// <summary>
        ///     load service
        /// </summary>
        /// <returns></returns>
        public virtual void LoadServices(List<Assembly> assemblies)
        {
            var jimuServicesType = GetJimuService(assemblies);
            var container = RegisterToIOC(jimuServicesType, assemblies);
            JimuServiceDescParser descParser = new JimuServiceDescParser();
            JimuServiceEntryParser entryParser = new JimuServiceEntryParser(container);
            var tmpServices = new List<JimuServiceEntry>();
            foreach (var type in jimuServicesType)
            {
                // the type catch from Interface.dll will not register sccessful, so we just filter the success one
                if (container.IsRegistered(type))
                {
                    foreach (var methodInfo in GetMethodInfos(type))
                    {
                        JimuServiceDesc desc = descParser.Parse(methodInfo);
                        var service = entryParser.Parse(methodInfo, desc);
                        tmpServices.Add(service);
                    }
                }

            }
            _services.Clear();
            _services.AddRange(tmpServices);

            OnServiceLoaded?.Invoke(_services);
        }

        private List<MethodInfo> GetMethodInfos(Type type)
        {
            var methods = type.GetTypeInfo().GetMethods().Where(x => x.GetCustomAttributes<JimuServiceDescAttribute>().Any()).ToList();
            if (!methods.Any() && !type.IsInterface)
            {
                type.GetInterfaces().Where(x => x.GetTypeInfo().IsAssignableTo<IJimuService>())
                    .ToList().ForEach(t => methods.AddRange(t.GetMethods().Where(m => m.GetCustomAttribute<JimuServiceAttribute>() != null)));
            }
            return methods;
        }
        private List<Type> GetJimuService(List<Assembly> assemblies)
        {
            List<Type> jimuServices = new List<Type>();
            assemblies.ForEach(x =>
                jimuServices.AddRange(x.ExportedTypes.Where(y => typeof(IJimuService).GetTypeInfo().IsAssignableFrom(y)).ToList())
            );
            return jimuServices;
        }

        private IContainer RegisterToIOC(List<Type> types, List<Assembly> assemblies)
        {
            //var serviceTypes = types.Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).Distinct();
            var containerBuilder = new ContainerBuilder();
            this._serviceRegisters.ForEach(x => x(containerBuilder));
            containerBuilder.RegisterTypes(types.ToArray()).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            containerBuilder.RegisterAssemblyModules(assemblies.ToArray());
            var container = containerBuilder.Build();
            this._serviceInitializers.ForEach(x => x(container));
            return container;
        }
        public List<JimuServiceEntry> GetServiceEntry()
        {
            return _services;
        }

        public void DoRegister(Action<ContainerBuilder> serviceRegister)
        {
            this._serviceRegisters.Add(serviceRegister);
        }

        public void DoInitializer(Action<IContainer> initializer)
        {
            this._serviceInitializers.Add(initializer);
        }
    }
}