using System.Collections.Generic;
using System.Threading.Tasks;
using Jimu.Core.Client.RemoteInvoker;

namespace Jimu.Client.Proxy.CodeAnalysisIntegration
{ 
    public abstract class ServiceProxyBase
    {
        private readonly IRemoteServiceInvoker _remoteServiceInvoker;

        protected ServiceProxyBase(IRemoteServiceInvoker remoteServiceInvoker)
        {
            _remoteServiceInvoker = remoteServiceInvoker;
        }

        protected async Task<T> Invoke<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return await _remoteServiceInvoker.Invoke<T>(serviceId, parameters);

        }

        protected T InvokeSync<T>(string serviceId, IDictionary<string, object> parameters)
        {
            return _remoteServiceInvoker.Invoke<T>(serviceId, parameters).Result;

        }

        protected async Task InvokeVoid(string serviceId, IDictionary<string, object> parameters)
        {
            await _remoteServiceInvoker.Invoke(serviceId, parameters);

        }

        protected void InvokeVoidSync(string serviceId, IDictionary<string, object> parameters)
        {
            _remoteServiceInvoker.Invoke(serviceId, parameters);

        }
    }
}
