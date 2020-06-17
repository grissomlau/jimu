using IService.User.dto;
using Jimu.Bus;
using Jimu.Logger;
using System.Threading.Tasks;

namespace Service.User.subscriber
{
    public class TestEventSubscriber : IJimuSubscriber<UserAdded>
    {
        ILogger _logger;
        public TestEventSubscriber(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(this.GetType());
        }
        public Task HandleAsync(IJimuSubscribeContext<UserAdded> context)
        {
            _logger.Debug($"event {context.Message.UserName}");
            return Task.CompletedTask;
        }
    }
}
