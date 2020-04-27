using IService.User.dto;
using Jimu.Core.Bus;
using Jimu.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.User.subscriber
{
    public class TestEventSubscriber : IJimuSubscriber<UserAdded>
    {
        ILogger _logger;
        public TestEventSubscriber(ILogger logger)
        {
            _logger = logger;
        }
        public Task HandleAsync(IJimuSubscribeContext<UserAdded> context)
        {
            _logger.Debug($"event {context.Message.UserName}");
            return Task.CompletedTask;
        }
    }
}
