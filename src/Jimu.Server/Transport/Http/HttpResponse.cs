using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Jimu.Server.Transport.Http
{
    public class HttpResponse : IResponse
    {
        readonly Microsoft.AspNetCore.Http.HttpResponse _httpResponse;
        private readonly ILogger _logger;
        public HttpResponse(Microsoft.AspNetCore.Http.HttpResponse httpResponse,  ILogger logger)
        {
            _httpResponse = httpResponse;
            _logger = logger;
        }
        public Task WriteAsync(string messageId, JimuRemoteCallResultData resultMessage)
        {
            _logger.Debug($"finish handling msg: {messageId}");
            var data = JimuHelper.Serialize<string>(new JimuTransportMsg(messageId, resultMessage));
            //_httpResponse.
            return _httpResponse.WriteAsync(data);

        }
    }
}
