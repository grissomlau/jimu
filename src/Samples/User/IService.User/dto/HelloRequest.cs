using Jimu.Bus;

namespace IService.User.dto
{
    public class HelloRequest : IJimuRequest
    {
        public string Greeting { get; set; }
        public string QueueName => "jimu-request-queue";
    }
}
