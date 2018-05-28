using System.Collections.Generic;
using System.Threading.Tasks;
using Jimu.Core.Protocols;

namespace Jimu.Core.Client.RemoteInvoker
{
    public interface IRemoteServiceInvoker
    {
        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters </param>
        /// <param name="token">authorization token</param>
        /// <returns></returns>
        Task<RemoteInvokeResultMessage> Invoke(string serviceIdOrPath, IDictionary<string, object> paras,
            string token = null);

        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <typeparam name="T">specify return type</typeparam>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters</param>
        /// <returns></returns>
        Task<T> Invoke<T>(string serviceIdOrPath, IDictionary<string, object> paras);
    }
}