using System.Threading.Tasks;
using Jimu;
using IServices;

namespace Services
{
    public class EchoService : IEchoService
    {

        private readonly ILogger _logger;
        private readonly JimuPayload _payload;
        public EchoService(ILogger logger, JimuPayload payload)
        {
            _logger = logger;
            _payload = payload;
        }

        public string GetEcho(string anything)
        {
            return $"the echo is  {anything}";
        }

        public Task<string> SetEcho(string anything)
        {
            return Task.FromResult($"the echo is {anything} and the current user  is {_payload.Items["username"]}");
        }
    }
}
