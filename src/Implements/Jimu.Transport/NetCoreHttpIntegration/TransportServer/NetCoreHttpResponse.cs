using System.Threading.Tasks;
using Jimu.Core.Commons.Logger;
using Jimu.Core.Commons.Serializer;
using Jimu.Core.Protocols;
using Jimu.Core.Server.TransportServer;
using Microsoft.AspNetCore.Http;

namespace Jimu.Common.Transport.NetCoreHttpIntegration.TransportServer
{
    public class NetCoreHttpResponse : IResponse
    {
        readonly HttpResponse _httpResponse;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        public NetCoreHttpResponse(HttpResponse httpResponse, ISerializer serializer, ILogger logger)
        {
            _httpResponse = httpResponse;
            _serializer = serializer;
            _logger = logger;
        }
        public Task WriteAsync(string messageId, RemoteInvokeResultMessage resultMessage)
        {
            _logger.Info($"结束处理消息： {messageId}");
            var data = _serializer.Serialize<string>(TransportMessage.Create(messageId, resultMessage));
            //_httpResponse.
            return _httpResponse.WriteAsync(data);

        }
    }
}
