using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.Discovery
{
    public interface IClientServiceDiscovery
    {
        void AddRoutesGetter(Func<Task<List<JimuServiceRoute>>> getter);
        /// <summary>
        ///     get all registered service routes
        /// </summary>
        /// <returns></returns>
        Task<List<JimuServiceRoute>> GetRoutesAsync();

        /// <summary>
        ///     get all registered service's server
        /// </summary>
        /// <returns></returns>
        Task<List<JimuAddress>> GetAddressAsync();

        Task UpdateServerHealthAsync(List<JimuAddress> addresses);
    }
}
