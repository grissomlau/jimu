using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jimu.Core.Client.LoadBalance;
using Jimu.Core.Client.TransportClient;
using Jimu.Core.Commons.Discovery;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Protocols;
using Jimu.Core.Server.ServiceContainer;

namespace Jimu.Core.Client.RemoteInvoker
{
    public class RemoteServiceInvoker : IRemoteServiceInvoker
    {
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger _logger;
        private readonly IServiceDiscovery _serviceDiscovery;
        private readonly IServiceTokenGetter _serviceTokenGetter;
        private readonly ITransportClientFactory _transportClientFactory;
        private readonly ITypeConvertProvider _typeConvertProvider;

        public RemoteServiceInvoker(IServiceDiscovery serviceDiscovery, IAddressSelector addressSelector,
            ITransportClientFactory transportClientFactory, ITypeConvertProvider typeConvertProvider,
            IServiceTokenGetter serviceTokenGetter, ILogger logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _addressSelector = addressSelector;
            _transportClientFactory = transportClientFactory;
            _typeConvertProvider = typeConvertProvider;
            _serviceTokenGetter = serviceTokenGetter;
            _logger = logger;
        }

        public async Task<T> Invoke<T>(string serviceIdOrPath, IDictionary<string, object> paras)
        {
            _logger.Info($"Invoking Service: {serviceIdOrPath}");
            var result = await Invoke(serviceIdOrPath, paras);
            if (!string.IsNullOrEmpty(result.ExceptionMessage)) throw new Exception(result.ExceptionMessage);
            if (result.Result == null) return default(T);
            object value;
            if (result.Result is Task<T> task)
                value = _typeConvertProvider.Convert(task.Result, typeof(T));
            else
                value = _typeConvertProvider.Convert(result.Result, typeof(T));
            return (T) value;

        }

        public async Task<RemoteInvokeResultMessage> Invoke(string serviceIdOrPath, IDictionary<string, object> paras,
            string token = null)
        {
            var service = await GetServiceById(serviceIdOrPath) ?? await GetServiceByPath(serviceIdOrPath, paras);
            if (service == null)
                return new RemoteInvokeResultMessage
                {
                    ErrorCode = "400",
                    ErrorMsg = $"{serviceIdOrPath}, not found!"
                };
            if (token == null && _serviceTokenGetter.GetToken != null) token = _serviceTokenGetter.GetToken();
            var result = await Invoke(service, paras, token);

            if (!string.IsNullOrEmpty(result.ExceptionMessage))
                return new RemoteInvokeResultMessage
                {
                    ErrorCode = "400",
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };

            if (string.IsNullOrEmpty(result.ErrorCode) && string.IsNullOrEmpty(result.ErrorMsg)) return result;
            if (int.TryParse(result.ErrorCode, out var erroCode) && erroCode > 200 && erroCode < 600)
                return new RemoteInvokeResultMessage
                {
                    ErrorCode = result.ErrorCode,
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };
            return result;

        }

        public async Task<RemoteInvokeResultMessage> Invoke(ServiceRoute service, IDictionary<string, object> paras,
            string token)
        {
            var address = await _addressSelector.GetAddressAsyn(service);
            var client = _transportClientFactory.CreateClient(address);
            if (client == null)
                return new RemoteInvokeResultMessage
                {
                    ErrorCode = "400",
                    ErrorMsg = "Server unavailable!"
                };
            _logger.Info(
                $"begin to invoke： serviceId:{service.ServiceDescriptor.Id},parameters count: {paras.Count()}, token:{token}");
            var result = await client.SendAsync(new RemoteInvokeMessage
            {
                Parameters = paras,
                ServiceId = service.ServiceDescriptor.Id,
                Token = token,
                Descriptor = service.ServiceDescriptor
            });
            return result;
        }

        private async Task<ServiceRoute> GetServiceByPath(string path, IDictionary<string, object> paras)
        {
            path = ParsePath(path, paras);
            var routes = await _serviceDiscovery.GetRoutesAsync();
            var service = routes.FirstOrDefault(x =>
                x.ServiceDescriptor.RoutePath.Equals(path, StringComparison.InvariantCultureIgnoreCase));
            return service;
        }

        private async Task<ServiceRoute> GetServiceById(string serviceId)
        {
            var routes = await _serviceDiscovery.GetRoutesAsync();
            var service = routes.FirstOrDefault(x =>
                x.ServiceDescriptor.Id.Equals(serviceId, StringComparison.InvariantCultureIgnoreCase));
            return service;
        }

        private static string ParsePath(string path, IDictionary<string, object> paras)
        {
            if (!paras.Any()) return path;
            path += "?";
            path = paras.Keys.Aggregate(path, (current, key) => current + (key + "=&"));
            path = path.TrimEnd('&');

            return path;
        }
    }
}