using Jimu.Logger;
using System;
using System.Collections.Generic;
using System.Text;
using User.IService;

namespace User.Service
{
    public class LoggerService : ILoggerService
    {
        readonly ILogger _logger;
        public LoggerService(ILogger logger)
        {
            _logger = logger;
        }
        public void Post(string log)
        {
            _logger.Debug(log);// will log in the folder log where in the root path of this server
        }
    }
}
