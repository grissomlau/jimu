using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Filter;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using log4net.Core;

namespace Jimu.Logger.Log4net
{
    public class Log4netLoggerFactory : ILoggerFactory
    {

        private readonly JimuLog4netOptions _options;
        string repositoryName = $"jimuLogger-{Guid.NewGuid()}";

        public Log4netLoggerFactory(JimuLog4netOptions options = null)
        {
            _options = options ?? new JimuLog4netOptions { EnableConsoleLog = true };
            var repLogger = LogManager.CreateRepository(repositoryName);
            if (!string.IsNullOrEmpty(options.Configuration))
            {
                XmlConfigurator.Configure(repLogger, new System.IO.FileInfo(options.Configuration));
            }
            else
            {
                if (_options.EnableFileLog)
                {
                    UseFileLogConfig((Hierarchy)repLogger, _options.FileLogLevelMax, _options.FileLogLevelMin);
                }
                if (_options.EnableConsoleLog)
                {
                    UseLogConsoleConfig((Hierarchy)repLogger, _options.FileLogLevelMax, _options.FileLogLevelMin);
                }
                //UseCodeConfig((Hierarchy)repLogger, LogLevel.Info);
                //UseCodeConfig((Hierarchy)repLogger, LogLevel.Warn);
                //UseCodeConfig((Hierarchy)repLogger, LogLevel.Error);
            }
        }

        void UseFileLogConfig(Hierarchy repository, LogLevel logLevelMax, LogLevel logLevelMin)
        {

            //var ip = JimuHelper.GetLocalIPAddress();
            PatternLayout layout = new PatternLayout
            {
                ConversionPattern = "%date{yyyy-MM-dd HH:mm:ss.fff} %-5p [%thread] %c %m%n"
            };
            layout.ActivateOptions();

            RollingFileAppender roller = new RollingFileAppender
            {
                AppendToFile = true,
            };
            var path = string.IsNullOrEmpty(_options.FileLogPath) ? "log" : _options.FileLogPath;
            roller.File = $@"{path}/";
            roller.PreserveLogFileNameExtension = true;
            roller.StaticLogFileName = false;
            roller.MaxSizeRollBackups = 0;
            roller.DatePattern = $@"yyyyMMdd"".log""";
            roller.RollingStyle = RollingFileAppender.RollingMode.Date;
            roller.Layout = layout;
            roller.MaxFileSize = 10000000;
            roller.AddFilter(new LevelRangeFilter()
            {
                LevelMax = ConvertToLevel(logLevelMax),
                LevelMin = ConvertToLevel(logLevelMin)
            });
            roller.ActivateOptions();
            repository.Root.AddAppender(roller);
            repository.Configured = true;
        }

        void UseLogConsoleConfig(Hierarchy repository, LogLevel logLevelMax, LogLevel logLevelMin)
        {

            ManagedColoredConsoleAppender console = new ManagedColoredConsoleAppender();
            PatternLayout layoutConsole = new PatternLayout
            {
                ConversionPattern = "%n%date{yyyy-MM-dd HH:mm:ss.fff} %-5level [%thread] %c %m",
            };
            console.AddFilter(new LevelRangeFilter() { LevelMax = ConvertToLevel(logLevelMax), LevelMin = ConvertToLevel(logLevelMin) });
            console.AddMapping(
                new ManagedColoredConsoleAppender.LevelColors { Level = Level.Error, ForeColor = ConsoleColor.DarkRed });
            console.AddMapping(
                new ManagedColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ConsoleColor.DarkYellow });

            layoutConsole.ActivateOptions();
            console.Layout = layoutConsole;
            console.ActivateOptions();
            repository.Root.AddAppender(console);

            repository.Configured = true;
        }

        public ILogger Create(Type type = null, [CallerFilePath] string path = "")
        {
            ILog log;
            if (type == null)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                log = LogManager.GetLogger(repositoryName, name);
            }
            else
            {
                log = LogManager.GetLogger(repositoryName, type);
            }
            return new Log4netLogger(log);
        }

        static Level ConvertToLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warn:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Error;
                default:
                    throw new LoggerException($"{logLevel} not found int log4net");
            }
        }
    }
}
