using System;
using System.Collections.Generic;
using Jimu.Core.Protocols;

namespace Jimu.Core.Server.ServiceContainer
{
    public interface IServiceEntryContainer
    {
        /// <summary>
        ///     get all service entries
        /// </summary>
        /// <returns></returns>
        List<ServiceEntry> GetServiceEntry();

        /// <summary>
        ///     add service
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IServiceEntryContainer AddServices(Type[] types);
    }
}