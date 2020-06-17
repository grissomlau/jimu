using System;
using N = NLog;

namespace Jimu.Logger.NLog
{
    public class NLogger : ILogger
    {
        private readonly N.Logger _logger;
        private readonly string _typeName;
        public NLogger(N.Logger logger, string typeName)
        {
            _logger = logger;
            _typeName = typeName;
        }
        public NLogger(N.Logger logger, Type type)
        {
            _logger = logger;
            _typeName = type.FullName;
        }

        public void Info(string info)
        {
            _logger.Info($"{_typeName} {info}");
        }

        public void Warn(string info)
        {
            _logger.Warn($"{_typeName} {info}");
        }

        public void Error(string info, Exception ex)
        {
            _logger.Error(ex, $"{_typeName} {info}");
        }

        public void Debug(string info)
        {
            _logger.Debug($"{_typeName} {info}");
        }
    }
}
