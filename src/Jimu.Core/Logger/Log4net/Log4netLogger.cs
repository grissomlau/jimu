using Jimu.Common;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Reflection;

namespace Jimu.Logger.Log4net
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _logger;
        private readonly JimuLog4netOptions _options;

        public Log4netLogger(JimuLog4netOptions options = null)
        {
            _options = options ?? new JimuLog4netOptions { EnableConsoleLog = true };
            string repositoryName = $"jimuLogger-{Guid.NewGuid()}";
            var repLogger = LogManager.CreateRepository(repositoryName);
            if (!string.IsNullOrEmpty(options.Configuration))
            {
                XmlConfigurator.Configure(repLogger, new System.IO.FileInfo(options.Configuration));
            }
            else
            {
                UseCodeConfig((Hierarchy)repLogger, LogLevel.Debug);
                UseCodeConfig((Hierarchy)repLogger, LogLevel.Info);
                UseCodeConfig((Hierarchy)repLogger, LogLevel.Warn);
                UseCodeConfig((Hierarchy)repLogger, LogLevel.Error);
            }
            _logger = LogManager.GetLogger(repositoryName, MethodBase.GetCurrentMethod().DeclaringType);
        }
        void UseCodeConfig(Hierarchy repository, LogLevel logLevel)
        {

            var ip = JimuHelper.GetLocalIPAddress();
            if (_options.EnableFileLog && (_options.FileLogLevel & logLevel) == logLevel)
            {
                PatternLayout layout = new PatternLayout
                {
                    ConversionPattern = "%date{yyyy-MM-dd HH:mm:ss.fff} %-5p [" + ip + "] %m%n"
                };
                layout.ActivateOptions();

                RollingFileAppender roller = new RollingFileAppender
                {
                    AppendToFile = true
                };
                var path = _options.EnableFileLog ? _options.FileLogPath : "log";
                roller.File = $@"{path}/{logLevel.ToString().ToLower()}/";
                roller.PreserveLogFileNameExtension = true;
                roller.StaticLogFileName = false;
                roller.MaxSizeRollBackups = 0;
                roller.DatePattern = $@"yyyyMMdd"".log""";
                roller.RollingStyle = RollingFileAppender.RollingMode.Date;
                roller.Layout = layout;
                roller.MaxFileSize = 10000000;
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        roller.Threshold = Level.Debug;
                        break;
                    case LogLevel.Info:
                        roller.Threshold = Level.Info;
                        break;
                    case LogLevel.Warn:
                        roller.Threshold = Level.Warn;
                        break;
                    case LogLevel.Error:
                        roller.Threshold = Level.Error;
                        break;
                }
                roller.ActivateOptions();
                repository.Root.AddAppender(roller);
            }

            if (_options.EnableConsoleLog && (_options.ConsoleLogLevel & logLevel) == logLevel)
            {

                ManagedColoredConsoleAppender console = new ManagedColoredConsoleAppender();
                PatternLayout layoutConsole = new PatternLayout
                {
                    ConversionPattern = "%n%date{yyyy-MM-dd HH:mm:ss.fff} %-5level [" + ip + "] %m",
                };
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        console.AddFilter(new LevelRangeFilter() { LevelMax = Level.Debug, LevelMin = Level.Debug });
                        break;
                    case LogLevel.Info:
                        console.AddFilter(new LevelRangeFilter() { LevelMax = Level.Info, LevelMin = Level.Info });
                        break;
                    case LogLevel.Warn:
                        console.AddFilter(new LevelRangeFilter() { LevelMax = Level.Warn, LevelMin = Level.Warn });
                        break;
                    case LogLevel.Error:
                        console.AddFilter(new LevelRangeFilter() { LevelMax = Level.Error, LevelMin = Level.Error });
                        break;
                }
                console.AddMapping(
                    new ManagedColoredConsoleAppender.LevelColors { Level = Level.Error, ForeColor = ConsoleColor.DarkRed });
                console.AddMapping(
                    new ManagedColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ConsoleColor.DarkYellow });

                layoutConsole.ActivateOptions();
                console.Layout = layoutConsole;
                console.ActivateOptions();
                repository.Root.AddAppender(console);
            }

            repository.Configured = true;
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
