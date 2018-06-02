using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jimu.Server
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
        public Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage)
        {
            _logger.Info($"finish handling： {messageId}");
            var data = _serializer.Serialize<string>(new JimuTransportMsg(messageId, resultMessage));
            //_httpResponse.
            return _httpResponse.WriteAsync(data);

        }
    }
}
