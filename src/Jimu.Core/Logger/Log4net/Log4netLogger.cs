using log4net;
using System;

namespace Jimu.Logger.Log4net
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _logger;
        public Log4netLogger(ILog logger)
        {
            _logger = logger;
        }

        public void Warn(string msg)
        {
            _logger.Warn(msg);
        }

        public void Error(string msg, Exception ex)
        {
            _logger.Error(msg, ex);

        }

        public void Debug(string info)
        {
            _logger.Debug(info);
        }

        public void Info(string msg)
        {
            _logger.Info(msg);
        }
    }
}
