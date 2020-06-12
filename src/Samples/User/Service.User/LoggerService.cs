using IService.User;
using Jimu;
using Jimu.Logger;

namespace Service.User
{
    public class LoggerService : ILoggerService
    {
        readonly ILogger _logger;
        public LoggerService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(this.GetType());
        }
        public void Post(string log)
        {
            _logger.Debug(log);// will log in the folder log where in the root path of this server
        }
    }
}
