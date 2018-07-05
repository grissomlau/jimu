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

namespace Jimu.Common.Logger
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly ILog _logWarn;
        private readonly LogOptions _options;

        public Log4netLogger(LogOptions options = null)
        {
            _options = options ?? new LogOptions { EnableConsoleLog = true, ConsoleLogLevel = LogLevel.Error | LogLevel.Info | LogLevel.Warn };
            var repInfo = LogManager.CreateRepository("info");
            var repWarn = LogManager.CreateRepository("warn");
            var repError = LogManager.CreateRepository("error");
            UseCodeConfig((Hierarchy)repInfo, "info");
            UseCodeConfig((Hierarchy)repError, "error");
            UseCodeConfig((Hierarchy)repError, "warn");
            _logInfo = LogManager.GetLogger("info", MethodBase.GetCurrentMethod().DeclaringType);
            _logError = LogManager.GetLogger("error", MethodBase.GetCurrentMethod().DeclaringType);
            _logWarn = LogManager.GetLogger("warn", MethodBase.GetCurrentMethod().DeclaringType);

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

        void UseCodeConfig(Hierarchy repository, string type)
        {

            if (_options.EnableFileLog)
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
                var path = _options.EnableFileLog ? _options.FileLogPath : "Log";
                roller.File = $@"{path}\{type}\";
                roller.StaticLogFileName = false;
                roller.MaxSizeRollBackups = 10;
                roller.DatePattern = @"yyyyMMdd"".log""";
                roller.RollingStyle = RollingFileAppender.RollingMode.Date;
                roller.Layout = layout;
                roller.ActivateOptions();

                repository.Root.AddAppender(roller);
            }

            if (_options.EnableConsoleLog)
            {

                //ManagedColoredConsoleAppender managedColoredConsoleAppender = new
                ManagedColoredConsoleAppender console = new ManagedColoredConsoleAppender();
                PatternLayout layoutConsole = new PatternLayout
                {
                    ConversionPattern = "%n%date{HH:mm:ss.fff} %-5level %m",
                };
                layoutConsole.ActivateOptions();
                console.Layout = layoutConsole;
                console.AddMapping(
                    new ManagedColoredConsoleAppender.LevelColors { Level = Level.Error, ForeColor = ConsoleColor.DarkRed });
                console.AddMapping(
                    new ManagedColoredConsoleAppender.LevelColors { Level = Level.Warn, ForeColor = ConsoleColor.DarkYellow });
                console.ActivateOptions();
                repository.Root.AddAppender(console);
            }

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            repository.Root.AddAppender(memory);

            repository.Root.Level = Level.Debug;
            repository.Configured = true;
        }

        public void Warn(string msg)
        {
            if ((_options.ConsoleLogLevel & LogLevel.Warn) == LogLevel.Warn)
            {
                _logError.Warn(msg);
            }
        }

        public void Error(string msg, Exception ex)
        {
            if ((_options.ConsoleLogLevel & LogLevel.Error) == LogLevel.Error)
                _logError.Error(msg, ex);

        }

        public void Info(string msg)
        {
            if ((_options.ConsoleLogLevel & LogLevel.Info) == LogLevel.Info)
                _logInfo.Info(msg);
        }
    }
}
