using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client
{
    public interface IRemoteServiceCaller
    {
        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters </param>
        /// <param name="token">authorization token</param>
        /// <returns></returns>
        Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras,
            string token = null);

        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <typeparam name="T">specify return type</typeparam>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters</param>
        /// <returns></returns>
        Task<T> InvokeAsync<T>(string serviceIdOrPath, IDictionary<string, object> paras);

        Task<JimuRemoteCallResultData> InvokeAsync(JimuServiceRoute service,
          IDictionary<string, object> paras,
          string token);
    }
}