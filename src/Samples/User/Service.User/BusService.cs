using IService.User;
using IService.User.dto;
using Jimu;
using Jimu.Core.Bus;
using Jimu.Logger;
using System.Threading.Tasks;

namespace Service.User
{
    public class BusService : IBusService
    {
        IJimuBus _bus;
        ILogger _logger;
        public BusService(IJimuBus bus, ILoggerFactory loggerFactory)
        {
            _bus = bus;
            _logger = loggerFactory.Create(this.GetType());
        }

        public Task<HelloResponse> Request(string greeting)
        {
            return _bus.RequestAsync<HelloRequest, HelloResponse>(new HelloRequest { Greeting = greeting });
        }

        public Task Send(UserReq req)
        {
            _logger.Info($"user name: {req.Name}");
            var addCommand = _bus.SendAsync(new AddUserCommand { UserName = req.Name });
            var addEvent = _bus.PublishAsync(new UserAdded { UserName = req.Name });
            Task.WaitAll(addCommand, addEvent);
            return Task.CompletedTask;
        }

    }
}
