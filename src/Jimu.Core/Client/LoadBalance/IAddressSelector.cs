using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.LoadBalance
{
    /// <summary>
    ///     server selector
    /// </summary>
    public interface IAddressSelector
    {
        /// <summary>
        ///     get server for specify route
        /// </summary>
        /// <param name="serviceRoute"></param>
        /// <returns></returns>
        Task<Address> GetAddressAsyn(ServiceRoute serviceRoute);
    }
}