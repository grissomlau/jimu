using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

namespace Jimu.Server.UnitOfWork.EF
{
    internal class SqlLogProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, SqlLogger> _loggers = new ConcurrentDictionary<string, SqlLogger>();

        readonly Action<LogLevel, string> _logAction;
        readonly LogLevel _logLevel;
        public SqlLogProvider(Action<LogLevel, string> logAction, LogLevel logLevel)
        {
            _logAction = logAction;
            _logLevel = logLevel;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new SqlLogger(categoryName, _logAction, _logLevel));
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
    }


    internal class SqlLogger : ILogger
    {
        readonly Action<LogLevel, string> _logAction;
        readonly string _categoryName;
        private LogLevel _logLevel;
        public SqlLogger(string categoryName, Action<LogLevel, string> logAction, LogLevel logLevel)
        {
            _logAction = logAction;
            _categoryName = categoryName;
            _logLevel = logLevel;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return (logLevel & _logLevel) == logLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel) && _logAction != null)
            {
                _logAction(logLevel, $"{_categoryName} - {eventId.Id} - {formatter(state, exception)}");
            }
            //Debug.WriteLine($"{logLevel.ToString()} - {eventId.Id} - {"haha"} - {formatter(state, exception)}");
        }
    }
}
