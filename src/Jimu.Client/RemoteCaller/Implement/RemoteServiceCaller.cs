using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jimu.Logger;
using Polly;

namespace Jimu.Client
{
    public class RemoteServiceCaller : IRemoteServiceCaller
    {
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger _logger;
        private readonly IClientServiceDiscovery _serviceDiscovery;
        private readonly IServiceTokenGetter _serviceTokenGetter;
        private readonly ClientSenderFactory _clientSenderFactory;
        private readonly Stack<Func<ClientRequestDel, ClientRequestDel>> _middlewares;

        public RemoteServiceCaller(IClientServiceDiscovery serviceDiscovery,
            IAddressSelector addressSelector,
            ClientSenderFactory clientSenderFactory,
            IServiceTokenGetter serviceTokenGetter,
            ILogger logger)
        {
            _serviceDiscovery = serviceDiscovery;
            _addressSelector = addressSelector;
            _clientSenderFactory = clientSenderFactory;
            _serviceTokenGetter = serviceTokenGetter;
            _logger = logger;
            _middlewares = new Stack<Func<ClientRequestDel, ClientRequestDel>>();
        }

        public async Task<T> InvokeAsync<T>(string serviceIdOrPath, IDictionary<string, object> paras)
        {
            _logger.Debug($"begin to invoke service: {serviceIdOrPath}");
            var result = await InvokeAsync(serviceIdOrPath, paras);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
            {
                _logger.Debug($"invoking service: {serviceIdOrPath} raising an error: {result.ExceptionMessage}");
                throw new Exception(result.ExceptionMessage);
            }
            if (!string.IsNullOrEmpty(result.ErrorCode))
            {
                _logger.Debug($"invoking service: {serviceIdOrPath} raising an error: errorcode {result.ErrorCode}, error: {result.ErrorMsg}");
                if (result.Result == null)
                {
                    return default(T);
                }
            }
            if (result.Result == null)
            {
                _logger.Debug($"invoking service: {serviceIdOrPath} has null return.");
                return default(T);
            }
            object value;
            if (result.Result is Task<T> task)
                value = JimuHelper.ConvertType(task.Result, typeof(T));
            else
                value = JimuHelper.ConvertType(result.Result, typeof(T));

            _logger.Debug($"finish invoking service: {serviceIdOrPath}.");
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
                    ErrorCode = "404",
                    ErrorMsg = $"{serviceIdOrPath}, not found!"
                };

            if (token == null && _serviceTokenGetter?.GetToken != null) token = _serviceTokenGetter.GetToken();
            var result = await InvokeAsync(service, paras, token);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
                return new JimuRemoteCallResultData
                {
                    ErrorCode = "500",
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
            var lastInvoke = GetLastInvoke();

            foreach (var mid in _middlewares)
            {
                lastInvoke = mid(lastInvoke);
            }

            return await lastInvoke(new RemoteCallerContext(service, paras, token, service.Address.First()));
        }

        private ClientRequestDel GetLastInvoke()
        {
            return new ClientRequestDel(async context =>
             {
                 var client = _clientSenderFactory.CreateClientSender(context.ServiceAddress);
                 if (client == null)
                     return new JimuRemoteCallResultData
                     {
                         ErrorCode = "400",
                         ErrorMsg = "Server unavailable!"
                     };
                 _logger.Debug($"invoke: serviceId:{context.Service.ServiceDescriptor.Id}, parameters count: {context.Paras.Count()}, token:{context.Token}");

                 return await client.SendAsync(new JimuRemoteCallData
                 {
                     Parameters = context.Paras,
                     ServiceId = context.Service.ServiceDescriptor.Id,
                     Payload = context.PayLoad,
                     Token = context.Token,
                     Descriptor = context.Service.ServiceDescriptor
                 });
             });
        }

        private async Task<JimuServiceRoute> GetServiceByPathAsync(string path, IDictionary<string, object> paras)
        {
            path = ParsePath(path, paras);
            var routes = await _serviceDiscovery.GetRoutesAsync();
            var service = routes.FirstOrDefault(x =>
                (x.ServiceDescriptor.RoutePath ?? "").Equals(path, StringComparison.InvariantCultureIgnoreCase));
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

        public IRemoteServiceCaller Use(Func<ClientRequestDel, ClientRequestDel> middleware)
        {
            this._middlewares.Push(middleware);
            return this;
        }
    }
}