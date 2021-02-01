using Jimu.Diagnostic;
using Jimu.Client.Diagnostic;
using Jimu.Client.Discovery;
using Jimu.Client.LoadBalance;
using Jimu.Client.Token;
using Jimu.Client.Transport;
using Jimu.Common;
using Jimu.Logger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jimu.Client.RemoteCaller.Implement
{
    public class RemoteServiceCaller : IRemoteServiceCaller
    {
        private readonly IAddressSelector _addressSelector;
        private readonly ILogger _logger;
        private readonly IClientServiceDiscovery _serviceDiscovery;
        private readonly IServiceTokenGetter _serviceTokenGetter;
        private readonly ClientSenderFactory _clientSenderFactory;
        private readonly Stack<Func<ClientRequestDel, ClientRequestDel>> _middlewares;
        private readonly IJimuDiagnostic _jimuApm;


        public RemoteServiceCaller(IClientServiceDiscovery serviceDiscovery,
            IAddressSelector addressSelector,
            ClientSenderFactory clientSenderFactory,
            IServiceTokenGetter serviceTokenGetter,
            IJimuDiagnostic jimuApm,
            ILoggerFactory loggerFactory)
        {
            _serviceDiscovery = serviceDiscovery;
            _addressSelector = addressSelector;
            _clientSenderFactory = clientSenderFactory;
            _serviceTokenGetter = serviceTokenGetter;
            _logger = loggerFactory.Create(this.GetType());
            _middlewares = new Stack<Func<ClientRequestDel, ClientRequestDel>>();
            _jimuApm = jimuApm;
        }

        public async Task<T> InvokeAsync<T>(string serviceIdOrPath, IDictionary<string, object> paras, JimuPayload payload)
        {
            _logger.Debug($"begin to invoke service: {serviceIdOrPath}");
            var result = await InvokeAsync(serviceIdOrPath, paras, payload);
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
        public async Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras, string httpMethod)
        {
            return await InvokeAsync(serviceIdOrPath, paras, null, null, httpMethod);

        }

        public async Task<JimuRemoteCallResultData> InvokeAsync(string serviceIdOrPath, IDictionary<string, object> paras, JimuPayload payload = null, string token = null, string httpMethod = null)
        {
            if (paras == null)
            {
                paras = new ConcurrentDictionary<string, object>();
            }
            JimuServiceRoute service = null;
            if (!string.IsNullOrEmpty(httpMethod))
            {
                service = await GetServiceByPathAsync(serviceIdOrPath, paras, httpMethod);
            }
            if (service == null)
            {
                service = await GetServiceByIdAsync(serviceIdOrPath);
            }
            if (service == null)
            {
                _logger.Debug($"{serviceIdOrPath} 404, service is null");
                return new JimuRemoteCallResultData
                {
                    ErrorCode = "404",
                    ErrorMsg = $"{serviceIdOrPath}, not found!"
                };
            }

            if (token == null && _serviceTokenGetter?.GetToken != null) token = _serviceTokenGetter.GetToken();
            var result = await InvokeAsync(service, paras, payload, token);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
            {
                return new JimuRemoteCallResultData
                {
                    ErrorCode = "500",
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };
            }

            if (string.IsNullOrEmpty(result.ErrorCode) && string.IsNullOrEmpty(result.ErrorMsg)) return result;
            if (int.TryParse(result.ErrorCode, out var erroCode) && erroCode > 200 && erroCode < 600)
                return new JimuRemoteCallResultData
                {
                    ErrorCode = result.ErrorCode,
                    ErrorMsg = $"{serviceIdOrPath}, {result.ToErrorString()}"
                };
            return result;

        }

        public async Task<JimuRemoteCallResultData> InvokeAsync(JimuServiceRoute service, IDictionary<string, object> paras, JimuPayload payload, string token)
        {
            var context = new RemoteCallerContext(service, paras, payload, token, service.Address.First());
            var operationId = _jimuApm.WriteRPCExecuteBefore(context);
            try
            {
                var lastInvoke = GetLastInvoke();

                foreach (var mid in _middlewares)
                {
                    lastInvoke = mid(lastInvoke);
                }
                var result = await lastInvoke(context);
                _jimuApm.WriteRPCExecuteAfter(operationId, context, result);
                return result;
            }
            catch (Exception ex)
            {
                _jimuApm.WriteRPCExecuteError(operationId, context, ex);
                throw;
            }
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
                     Payload = context.Payload,
                     Token = context.Token,
                     Descriptor = context.Service.ServiceDescriptor
                 });
             });
        }

        private async Task<JimuServiceRoute> GetServiceByPathAsync(string path, IDictionary<string, object> paras, string httpMethod)
        {
            //path = ParsePath(path, paras);
            var routes = await _serviceDiscovery.GetRoutesAsync();

            var service = routes.FirstOrDefault(x =>
            {
                if (httpMethod != x.ServiceDescriptor.HttpMethod)
                    return false;

                //var restPathPattern = $"^{Regex.Replace(x.ServiceDescriptor.RestPath.Replace("?", "[?]"), @"{([^{}?/\\]+)}", @"(?<$1>[^?/\\]+)", RegexOptions.IgnoreCase)}$";
                var restPathPattern = $"^{Regex.Replace(x.ServiceDescriptor.RoutePath.Replace("?", "[?]"), @"{([^{}?/\\]+)}", @"(?<$1>[^?/\\]+)", RegexOptions.IgnoreCase)}$";
                var matches = Regex.Matches((string)path, restPathPattern, RegexOptions.IgnoreCase);
                // has parameters
                if (matches.Count > 0 && matches[0].Groups.Count > 1)
                {
                    for (var i = 1; i < matches[0].Groups.Count; ++i)
                    {
                        //System.Text.RegularExpressions.MatchEvaluator matchEvaluator = new MatchEvaluator();
                        Group g = matches[0].Groups[i];
                        if (!paras.ContainsKey(g.Name))
                            paras.Add(g.Name, g.Value);
                    }
                }
                return matches.Count > 0;
            }
                 );

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
            path = paras.Keys.Aggregate(path, (current, key) => $"{current}{key}={{{key}}}");
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