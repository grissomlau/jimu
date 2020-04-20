using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Jimu.Server.Transport
{
    public delegate Task RequestDel(ServiceInvokerContext context);
    /// <summary>
    ///     transport server
    /// </summary>
    public interface IServer
    {
        /// <summary>
        ///     get all the service in this server
        /// </summary>
        /// <returns></returns>
        List<JimuServiceRoute> GetServiceRoutes();

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

        IPEndPoint GetServerAddress();
    }
}