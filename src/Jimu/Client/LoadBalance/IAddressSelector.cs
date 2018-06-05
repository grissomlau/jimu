using System.Threading.Tasks;

namespace Jimu.Client
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
        Task<JimuAddress> GetAddressAsyn(JimuServiceRoute serviceRoute);
    }
}