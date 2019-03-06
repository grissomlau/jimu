using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Jimu.Logger
{
    public class Log4netLogger : ILogger
    {
        //private readonly ILog _logDebug;
        //private readonly ILog _logInfo;
        //private readonly ILog _logError;
        //private readonly ILog _logWarn;
        private readonly ILog _logger;
        private readonly LogOptions _options;

        public Log4netLogger(LogOptions options = null)
        {
            _options = options ?? new LogOptions { EnableConsoleLog = true };
            var repLogger = LogManager.CreateRepository("jimuLogger");
            //var repDebug = LogManager.CreateRepository("debug");
            //var repInfo = LogManager.CreateRepository("info");
            //var repWarn = LogManager.CreateRepository("warn");
            //var repError = LogManager.CreateRepository("error");
            //UseCodeConfig((Hierarchy)repDebug, "debug");
            //UseCodeConfig((Hierarchy)repInfo, "info");
            //UseCodeConfig((Hierarchy)repError, "error");
            //UseCodeConfig((Hierarchy)repWarn, "warn");
            UseCodeConfig((Hierarchy)repLogger, LogLevel.Debug);
            UseCodeConfig((Hierarchy)repLogger, LogLevel.Info);
            UseCodeConfig((Hierarchy)repLogger, LogLevel.Warn);
            UseCodeConfig((Hierarchy)repLogger, LogLevel.Error);
            _logger = LogManager.GetLogger("jimuLogger", MethodBase.GetCurrentMethod().DeclaringType);
            //_logDebug = LogManager.GetLogger("debug", MethodBase.GetCurrentMethod().DeclaringType);
            //_logInfo = LogManager.GetLogger("info", MethodBase.GetCurrentMethod().DeclaringType);
            //_logError = LogManager.GetLogger("error", MethodBase.GetCurrentMethod().DeclaringType);
            //_logWarn = LogManager.GetLogger("warn", MethodBase.GetCurrentMethod().DeclaringType);

        }
        //void UseFileCofnig(ILoggerRepository rep)
        //{
        //    var type = MethodBase.GetCurrentMethod().DeclaringType;
        //    if (type != null)
        //    {
        //        var ns = type.Namespace;
        //        Assembly assembly = Assembly.GetExecutingAssembly();
        //        string resourceName = ns + ".log4net.config";
        //        Stream stream = assembly.GetManifestResourceStream(resourceName);
        //        XmlConfigurator.Configure(rep, stream);
        //    }
        //}

        //void UseCodeConfig(Hierarchy repository, string type)
        void UseCodeConfig(Hierarchy repository, LogLevel logLevel)
        {

            if (_options.EnableFileLog && (_options.FileLogLevel & logLevel) == logLevel)
            {
                PatternLayout layout = new PatternLayout
                {
                    ConversionPattern = "%date{yyyy-MM-dd HH:mm:ss.fff} %-5p %m%n"
                };
                layout.ActivateOptions();

                RollingFileAppender roller = new RollingFileAppender
                {
                    AppendToFile = false
                };
                var path = _options.EnableFileLog ? _options.FileLogPath : "log";
                roller.File = $@"{path}\{logLevel.ToString().ToLower()}\";
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

                //ManagedColoredConsoleAppender managedColoredConsoleAppender = new
                ManagedColoredConsoleAppender console = new ManagedColoredConsoleAppender();
                PatternLayout layoutConsole = new PatternLayout
                {
                    ConversionPattern = "%n%date{yyyy-MM-dd HH:mm:ss.fff} %-5level %m",
                };
                switch (logLevel)
                {
                    case LogLevel.Debug:
                        console.Threshold = Level.Debug;
                        break;
                    case LogLevel.Info:
                        console.Threshold = Level.Info;
                        break;
                    case LogLevel.Warn:
                        console.Threshold = Level.Warn;
                        break;
                    case LogLevel.Error:
                        console.Threshold = Level.Error;
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

            //MemoryAppender memory = new MemoryAppender();
            //memory.ActivateOptions();
            //repository.Root.AddAppender(memory);

            //repository.Root.Level = Level.Debug;
            repository.Configured = true;
        }

        public void Warn(string msg)
        {
            _logger.Warn(msg);
        }

        public void Error(string msg, Exception ex)
        {
            //if ((_options.ConsoleLogLevel & LogLevel.Error) == LogLevel.Error)
            //    _logError.Error(msg, ex);
            _logger.Error(msg, ex);

        }

        public void Debug(string info)
        {
            //if ((_options.ConsoleLogLevel & LogLevel.Debug) == LogLevel.Debug)
            //    _logDebug.Debug(info);
            _logger.Debug(info);
        }

        public void Info(string msg)
        {
            //if ((_options.ConsoleLogLevel & LogLevel.Info) == LogLevel.Info)
            //    _logInfo.Info(msg);
            _logger.Info(msg);
        }
    }
}
