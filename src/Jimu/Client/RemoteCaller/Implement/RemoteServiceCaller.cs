using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Polly;

namespace Jimu.Client
{
    public class RemoteServiceCaller : IRemoteServiceCaller
    {
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger _logger;
        private readonly IClientServiceDiscovery _serviceDiscovery;
        private readonly IServiceTokenGetter _serviceTokenGetter;
        private readonly ITransportClientFactory _transportClientFactory;
        private readonly ITypeConvertProvider _typeConvertProvider;
        private int _retryTimes;

        public RemoteServiceCaller(IClientServiceDiscovery serviceDiscovery, IAddressSelector addressSelector,
            ITransportClientFactory transportClientFactory, ITypeConvertProvider typeConvertProvider,
            IServiceTokenGetter serviceTokenGetter, ILogger logger, int retryTimes = -1)
        {
            _serviceDiscovery = serviceDiscovery;
            _addressSelector = addressSelector;
            _transportClientFactory = transportClientFactory;
            _typeConvertProvider = typeConvertProvider;
            _serviceTokenGetter = serviceTokenGetter;
            _logger = logger;
            _retryTimes = retryTimes;
        }

        public async Task<T> InvokeAsync<T>(string serviceIdOrPath, IDictionary<string, object> paras)
        {
            _logger.Info($"Invoking Service: {serviceIdOrPath}");
            var result = await InvokeAsync(serviceIdOrPath, paras);
            if (!string.IsNullOrEmpty(result.ExceptionMessage)) throw new Exception(result.ExceptionMessage);
            if (result.Result == null) return default(T);
            object value;
            if (result.Result is Task<T> task)
                value = _typeConvertProvider.Convert(task.Result, typeof(T));
            else
                value = _typeConvertProvider.Convert(result.Result, typeof(T));
            return (T)value;

        }

        public async Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras,
            string token = null)
        {
            if (paras == null)
            {
                paras = new ConcurrentDictionary<string, object>();
            }
            var service = await GetServiceByIdAsync(serviceIdOrPath) ?? await GetServiceByPathAsync(serviceIdOrPath, paras);
            if (service == null)
                return new JimuRemoteCallResultData
                {
                    ErrorCode = "400",
                    ErrorMsg = $"{serviceIdOrPath}, not found!"
                };

            if (token == null && _serviceTokenGetter.GetToken != null) token = _serviceTokenGetter.GetToken();
            var result = await InvokeAsync(service, paras, token);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
                return new JimuRemoteCallResultData
                {
                    ErrorCode = "400",
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };

            if (string.IsNullOrEmpty(result.ErrorCode) && string.IsNullOrEmpty(result.ErrorMsg)) return result;
            if (int.TryParse(result.ErrorCode, out var erroCode) && erroCode > 200 && erroCode < 600)
                return new JimuRemoteCallResultData
                {
                    ErrorCode = result.ErrorCode,
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };
            return result;

        }

        public async Task<JimuRemoteCallResultData> InvokeAsync(JimuServiceRoute service, IDictionary<string, object> paras,
            string token)
        {
            if (paras == null) paras = new ConcurrentDictionary<string, object>();
            var address = await _addressSelector.GetAddressAsyn(service);
            if (_retryTimes < 0)
            {
                _retryTimes = service.Address.Count;
            }
            JimuRemoteCallResultData result = null;
            var retryPolicy = Policy.Handle<TransportException>()
                .RetryAsync(_retryTimes,
                    async (ex, count) =>
                    {
                        address = await _addressSelector.GetAddressAsyn(service);
                        _logger.Info(
                            $"FaultHandling,retry times: {count},serviceId: {service.ServiceDescriptor.Id},Address: {address.Code},RemoteServiceCaller excute retry by Polly for exception {ex.Message}");
                    });
            var fallbackPolicy = Policy<JimuRemoteCallResultData>.Handle<TransportException>()
                .FallbackAsync(new JimuRemoteCallResultData() { ErrorCode = "500", ErrorMsg = "error occur when communicate with server. server maybe have been down." })
                .WrapAsync(retryPolicy);
            return await fallbackPolicy.ExecuteAsync(async () =>
                    {
                        var client = _transportClientFactory.CreateClient(address);
                        if (client == null)
                            return new JimuRemoteCallResultData
                            {
                                ErrorCode = "400",
                                ErrorMsg = "Server unavailable!"
                            };
                        _logger.Info($"begin to invoke： serviceId:{service.ServiceDescriptor.Id},parameters count: {paras.Count()}, token:{token}");
                        //Polly.Policy.Handle<>()

                        result = await client.SendAsync(new JimuRemoteCallData
                        {
                            Parameters = paras,
                            ServiceId = service.ServiceDescriptor.Id,
                            Token = token,
                            Descriptor = service.ServiceDescriptor
                        });
                        return result;
                    });
        }

        private async Task<JimuServiceRoute> GetServiceByPathAsync(string path, IDictionary<string, object> paras)
        {
            path = ParsePath(path, paras);
            var routes = await _serviceDiscovery.GetRoutesAsync();
            var service = routes.FirstOrDefault(x =>
                x.ServiceDescriptor.RoutePath.Equals(path, StringComparison.InvariantCultureIgnoreCase));
            return service;
        }

        private async Task<JimuServiceRoute> GetServiceByIdAsync(string serviceId)
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