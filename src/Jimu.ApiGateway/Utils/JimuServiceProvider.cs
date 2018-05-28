using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Jimu.ApiGateway.Model;
using Jimu.Common.Logger.Log4netIntegration;
using Jimu.Core;
using Jimu.Core.Client;
using Jimu.Core.Client.LoadBalance;
using Jimu.Core.Client.RemoteInvoker;
using Jimu.Core.Commons.Serializer;
using Jimu.Core.Protocols;

namespace Jimu.ApiGateway.Utils
{
    public class JimuServiceProvider
    {
        public static IContainer Container { get; private set; }
        private static IServiceHost _host;

        public static void Start(ContainerBuilder containerBuilder, JimuOptions mServiceOptions)
        {
            _host = new ServiceHostClientBuilder(containerBuilder)
                .UseLog4netLogger(new Log4netOptions
                {
                    EnableConsoleLog = true
                })
                .UseConsul(mServiceOptions.ConsulIp, mServiceOptions.ConsulPort, mServiceOptions.ServiceCategory)
                .UseDotNettyClient()
                .UseNetCoreHttpClient()
                .UsePollingAddressSelector()
                .UseServerHealthCheck(1)
                .UseRemoteServiceInvoker(() => { var headers = JimuHttpContext.Current.Request.Headers["Authorization"]; return headers.Any() ? headers[0] : null; })
                .Build();
            Container = _host.Container;
        }

        //public static async Task<object> Invoke(string path, IDictionary<string, object> paras, string token)
        //public static async Task<object> Invoke(HttpResponse httpResponse, string path, IDictionary<string, object> paras)
        public static async Task<RemoteInvokeResultMessage> Invoke( string path, IDictionary<string, object> paras)
        {
            //path = ParsePath(path, paras);
            var remoteServiceInvoker = _host.Container.Resolve<IRemoteServiceInvoker>();
            var converter = _host.Container.Resolve<ISerializer>();
            //var serviceDiscovery = _host.Container.Resolve<IServiceDiscovery>();
            //var addressSelector = _host.Container.Resolve<IAddressSelector>();
            //var routes = await serviceDiscovery.GetRoutesAsync();
            //var service = routes.FirstOrDefault(x => x.ServiceDescriptor.RoutePath.Equals(path, StringComparison.InvariantCultureIgnoreCase));
            //if (service != null)
            //{
            //var clientFactory = _host.Container.Resolve<ITransportClientFactory>();
            //var address = await addressSelector.GetAddressAsyn(service);
            //var client = clientFactory.CreateClient(address.CreateEndPoint());
            //if (client == null)
            //{

            //    throw new HttpStatusCodeException(400, "Server unavailable!");
            //}
            //var result = await client.SendAsync(new RemoteInvokeMessage
            //{
            //    Parameters = paras,
            //    ServiceId = service.ServiceDescriptor.Id,
            //    Token = token

            //});
            //var result = await remoteServiceInvoker.Invoke(path, paras, token);
            var result = await remoteServiceInvoker.Invoke(path, paras);
            if (!string.IsNullOrEmpty(result.ExceptionMessage))
            {
                throw new HttpStatusCodeException(400, $"{path}, {result.ToErrorString()}");
            }

            if (!string.IsNullOrEmpty(result.ErrorCode) || !string.IsNullOrEmpty(result.ErrorMsg))
            {
                if (int.TryParse(result.ErrorCode, out int erroCode) && erroCode > 200 && erroCode < 600)
                {
                    throw new HttpStatusCodeException(erroCode, result.ToErrorString());
                }

                return new RemoteInvokeResultMessage { ErrorCode = result.ErrorCode, ErrorMsg = result.ErrorMsg };
            }
            if (result.ResultType == typeof(FileModel).ToString())
            {
                var file = converter.Deserialize(result.Result, typeof(FileModel));
                result.Result = file;
            }

            return result;

            //}
            //throw new HttpStatusCodeException(404, $"{path}, not found!");
        }

    }
}
