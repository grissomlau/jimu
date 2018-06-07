using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jimu.Client.Proxy.CodeAnalysisIntegration
{
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteServiceCaller _remoteServiceCaller;

        protected ServiceProxyBase(IRemoteServiceCaller remoteServiceCaller)
        {
            _remoteServiceCaller = remoteServiceCaller;
        }

        protected async Task<T> InvokeAsync<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return await _remoteServiceCaller.InvokeAsync<T>(serviceId, parameters);

        }

        protected T Invoke<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return _remoteServiceCaller.InvokeAsync<T>(serviceId, parameters).Result;

        }

        protected async Task InvokeVoidAsync(string serviceId, IDictionary<string, object> parameters)
        {
            await _remoteServiceCaller.InvokeAsync(serviceId, parameters);

        }

        protected void InvokeVoid(string serviceId, IDictionary<string, object> parameters)
        {
            _remoteServiceCaller.InvokeAsync(serviceId, parameters);

        }
    }
}
