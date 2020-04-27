using IService.User.dto;
using Jimu.Core.Bus;
using Jimu.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.User.consumer
{
    public class AddUserConsumer : IJimuConsumer<AddUserCommand>
    {

        ILogger _logger;
        public AddUserConsumer(ILogger logger)
        {
            _logger = logger;
        }
        public Task ConsumeAsync(IJimuConsumeContext<AddUserCommand> context)
        {
            _logger.Info($"consumer, user name : {context.Message.UserName}");
            return Task.CompletedTask;
        }
    }
}
