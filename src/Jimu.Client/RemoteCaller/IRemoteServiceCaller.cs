using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.RemoteCaller
{
    public delegate Task<JimuRemoteCallResultData> ClientRequestDel(RemoteCallerContext context);
    public interface IRemoteServiceCaller
    {
        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters </param>
        /// <param name="httpMethod">httpMethod </param>
        /// <returns></returns>
        Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras, string httpMethod);
        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters </param>
        /// <param name="payload">context payload</param>
        /// <param name="token">authorization token</param>
        /// <param name="httpMethod">httpMethod </param>
        /// <returns></returns>
        Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras, JimuPayload payload = null, string token = null, string httpMethod = null);

        /// <summary>
        ///     invoke remote service
        /// </summary>
        /// <typeparam name="T">specify return type</typeparam>
        /// <param name="serviceIdOrPath">service id or route path</param>
        /// <param name="paras">service parameters</param>
        /// <param name="payload">context payload</param>
        /// <returns></returns>
        Task<T> InvokeAsync<T>(string serviceIdOrPath, IDictionary<string, object> paras, JimuPayload payload);

        Task<JimuRemoteCallResultData> InvokeAsync(JimuServiceRoute service, IDictionary<string, object> paras, JimuPayload payload, string token);


        /// <summary>
        ///     add middleware in request
        /// </summary>
        /// <param name="middleware"></param>
        /// <returns></returns>
        IRemoteServiceCaller Use(Func<ClientRequestDel, ClientRequestDel> middleware);

        Task<bool> ExistAsync(string serviceIdOrPath, string httpMethod, IDictionary<string, object> paras);
    }
}