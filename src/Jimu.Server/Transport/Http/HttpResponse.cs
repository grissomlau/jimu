using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jimu.Server.Transport.Http
{
    public class HttpResponse : IResponse
    {
        readonly Microsoft.AspNetCore.Http.HttpResponse _httpResponse;
        private readonly ISerializer _serializer;
        private readonly ILogger _logger;
        public HttpResponse(Microsoft.AspNetCore.Http.HttpResponse httpResponse, ISerializer serializer, ILogger logger)
        {
            _httpResponse = httpResponse;
            _serializer = serializer;
            _logger = logger;
        }
        public Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage)
        {
            _logger.Info($"finish handling msg: {messageId}");
            var data = _serializer.Serialize<string>(new JimuTransportMsg(messageId, resultMessage));
            //_httpResponse.
            return _httpResponse.WriteAsync(data);

        }
    }
}
