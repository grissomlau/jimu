using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Server.Discovery
{
    /// <summary>
    ///     how to discovery service
    /// </summary>
    public interface IServiceDiscovery
    {
        /// <summary>
        ///     clear all discovered service
        /// </summary>
        /// <returns></returns>
        Task ClearAsync();

        /// <summary>
        ///     clear specified server discovered service
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        //Task ClearAsync(string address);

        /// <summary>
        ///     clear discovered service by specify service id
        /// </summary>
        /// <param name="serviceId"></param>
        /// <returns></returns>
        Task ClearServiceAsync(string serviceId);

        /// <summary>
        ///     register service by service routes
        /// </summary>
        /// <param name="routes"></param>
        /// <returns></returns>
        Task SetRoutesAsync(List<JimuServiceRoute> routes);
        /// <summary>
        /// register service by service route
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        //Task AddRouteAsync(List<JimuServiceRoute> routes);

        /// <summary>
        ///     get all registered service routes
        /// </summary>
        /// <returns></returns>
        Task<List<JimuServiceRoute>> GetRoutesAsync();
        /// <summary>
        /// refresh remote discovery server
        /// </summary>
        /// <returns></returns>
        Task<List<JimuServiceRoute>> RefreshRoutesAsync();

        /// <summary>
        ///     get all registered service's server
        /// </summary>
        /// <returns></returns>
        Task<List<JimuAddress>> GetAddressAsync();

        event Action<List<JimuServiceRoute>> OnBeforeSetRoutes;
    }
}