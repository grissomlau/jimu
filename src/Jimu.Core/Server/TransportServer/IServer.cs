using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Server.TransportServer
{
    /// <summary>
    ///     transport server
    /// </summary>
    public interface IServer
    {
        /// <summary>
        ///     get all the service in this server
        /// </summary>
        /// <returns></returns>
        List<ServiceRoute> GetServiceRoutes();

        /// <summary>
        ///     start the server
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        ///     add middleware in response
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        IServer Use(Func<RequestDel, RequestDel> middleware);
    }
}