using IService.User.dto;
using Jimu.Core.Bus;
using System.Threading.Tasks;

namespace Service.User.requestHandler
{
    public class HelloRequestHanlder : IJimuRequestHandler<HelloRequest, HelloResponse>
    {
        public Task<HelloResponse> HandleAsync(IJimuRequestContext<HelloRequest> context)
        {
            return Task.FromResult(new HelloResponse { Greeting = $"Jimu {context.Message.Greeting}" });
        }
    }
}
