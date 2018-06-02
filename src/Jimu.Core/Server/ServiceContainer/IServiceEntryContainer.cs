using System;
using System.Collections.Generic;

namespace Jimu.Server
{
    public interface IServiceEntryContainer
    {
        /// <summary>
        ///     get all service entries
        /// </summary>
        /// <returns></returns>
        List<JimuServiceEntry> GetServiceEntry();

        /// <summary>
        ///     add service
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        IServiceEntryContainer AddServices(Type[] types);
    }
}