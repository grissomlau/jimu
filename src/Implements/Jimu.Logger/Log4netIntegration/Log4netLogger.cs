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

namespace Jimu.Common.Logger.Log4netIntegration
{
    public class Log4netLogger : ILogger
    {
        private readonly ILog _logInfo;
        private readonly ILog _logError;
        private readonly Log4netOptions _options;

        public Log4netLogger(Log4netOptions options)
        {
            _options = options;
            var repInfo = LogManager.CreateRepository("info");
            var repError = LogManager.CreateRepository("error");
            UseCodeConfig((Hierarchy)repInfo, "info");
            UseCodeConfig((Hierarchy)repError, "error");
            _logInfo = LogManager.GetLogger("info", MethodBase.GetCurrentMethod().DeclaringType);
            _logError = LogManager.GetLogger("error", MethodBase.GetCurrentMethod().DeclaringType);

        }
        void UseFileCofnig(ILoggerRepository rep)
        {
            var type = MethodBase.GetCurrentMethod().DeclaringType;
            if (type != null)
            {
                var ns = type.Namespace;
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = ns + ".log4net.config";
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                XmlConfigurator.Configure(rep, stream);
            }
        }

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

                ConsoleAppender console = new ConsoleAppender();
                PatternLayout layoutConsole = new PatternLayout
                {
                    ConversionPattern = "%n%date{HH:mm:ss.fff} %-5level %m"
                };
                layoutConsole.ActivateOptions();
                console.Layout = layoutConsole;
                repository.Root.AddAppender(console);
            }

            MemoryAppender memory = new MemoryAppender();
            memory.ActivateOptions();
            repository.Root.AddAppender(memory);

            repository.Root.Level = Level.Debug;
            repository.Configured = true;
        }

        public void Error(string msg, Exception ex)
        {
            if ((_options.LogLevel & LogLevel.Error) == LogLevel.Error)
                _logError.Error(msg, ex);

        }

        public void Info(string msg)
        {
            if ((_options.LogLevel & LogLevel.Info) == LogLevel.Info)
                _logInfo.Info(msg);
        }
    }
}
