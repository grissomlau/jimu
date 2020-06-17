using IService.User.dto;
using Jimu.Bus;
using Jimu.Logger;
using System.Threading.Tasks;

namespace Service.User.consumer
{
    public class AddUserConsumer : IJimuConsumer<AddUserCommand>
    {

        ILogger _logger;
        public AddUserConsumer(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(this.GetType());
        }
        public Task ConsumeAsync(IJimuConsumeContext<AddUserCommand> context)
        {
            _logger.Info($"consumer, user name : {context.Message.UserName}");
            return Task.CompletedTask;
        }
    }
}
