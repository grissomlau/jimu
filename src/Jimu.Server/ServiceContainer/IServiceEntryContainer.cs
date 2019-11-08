using Autofac;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jimu.Server.ServiceContainer
{
    public interface IServiceEntryContainer
    {
        void DoRegister(Action<ContainerBuilder> serviceRegister);
        void DoInitializer(Action<IContainer> initializer);

        event Action<List<JimuServiceEntry>> OnServiceLoaded;

        /// <summary>
        ///     load service
        /// </summary>
        /// <returns></returns>
        void LoadServices(List<Assembly> assemblies);
        /// <summary>
        ///     get all service entries
        /// </summary>
        /// <returns></returns>
        List<JimuServiceEntry> GetServiceEntry();
    }
}