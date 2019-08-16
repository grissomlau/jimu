using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.Proxy.CodeAnalysisIntegration
{
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteServiceCaller _remoteServiceCaller;
        private readonly JimuPayload _payload;

        protected ServiceProxyBase(IRemoteServiceCaller remoteServiceCaller, JimuPayload payload)
        {
            _remoteServiceCaller = remoteServiceCaller;
            this._payload = payload;
        }

        protected async Task<T> InvokeAsync<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return await _remoteServiceCaller.InvokeAsync<T>(serviceId, parameters, _payload);

        }

        protected T Invoke<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return _remoteServiceCaller.InvokeAsync<T>(serviceId, parameters, _payload).Result;

        }

        protected async Task InvokeVoidAsync(string serviceId, IDictionary<string, object> parameters)
        {
            await _remoteServiceCaller.InvokeAsync(serviceId, parameters, _payload);

        }

        protected void InvokeVoid(string serviceId, IDictionary<string, object> parameters)
        {
            _remoteServiceCaller.InvokeAsync(serviceId, parameters, _payload);

        }
    }
}
