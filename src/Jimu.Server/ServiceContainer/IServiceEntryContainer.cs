using System;
using System.Collections.Generic;
using System.Reflection;

namespace Jimu.Server
{
    public interface IServiceEntryContainer
    {

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