using System;
using System.Collections.Generic;
using System.Text;
using NLog;
using NLog.Targets;

namespace Jimu
{
    public class NLogger : ILogger
    {
        private readonly NLog.Logger _logger;

        public NLogger(LogOptions options = null)
        {
            options = options ?? new LogOptions { EnableConsoleLog = true };
            var config = new NLog.Config.LoggingConfiguration();
            if (options.EnableFileLog)
            {
                var fileConf = new NLog.Targets.FileTarget("jimuLogFile")
                {
                    FileName = ".\\log\\${level:lowercase=true}\\${shortdate}.log",
                    ArchiveAboveSize = 10000000,
                    Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true}  ${message}"
                };
                if(options.FileLogPath!= null)
                {
                    fileConf.FileName = options.FileLogPath+"\\${level:lowercase=true}\\${shortdate}.log";

                }
                if ((options.FileLogLevel & LogLevel.Error) == LogLevel.Error)
                {
                    config.AddRuleForOneLevel(NLog.LogLevel.Error, fileConf);
                }
                if ((options.FileLogLevel & LogLevel.Warn) == LogLevel.Warn)
                {
                    //config.AddRuleForOneLevel(NLog.LogLevel.Warn, fileConf);
                    config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Error, fileConf);
                }
                if ((options.FileLogLevel & LogLevel.Info) == LogLevel.Info)
                {
                    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Error, fileConf);
                    //config.AddRuleForOneLevel(NLog.LogLevel.Info, fileConf);
                }
                if ((options.FileLogLevel & LogLevel.Debug) == LogLevel.Debug)
                {
                    config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Error, fileConf);
                    //config.AddRuleForOneLevel(NLog.LogLevel.Debug, fileConf);
                }
            }

            if (options.EnableConsoleLog)
            {
                var consoleLog = new NLog.Targets.ConsoleTarget("jimuLogconsole")
                {
                    Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true}  ${message}"
                };
                if ((options.ConsoleLogLevel & LogLevel.Error) == LogLevel.Error)
                {
                    config.AddRuleForOneLevel(NLog.LogLevel.Error, consoleLog);
                }
                if ((options.ConsoleLogLevel & LogLevel.Warn) == LogLevel.Warn)
                {
                    config.AddRule(NLog.LogLevel.Warn, NLog.LogLevel.Error, consoleLog);
                    //config.AddRuleForOneLevel(NLog.LogLevel.Warn, consoleLog);
                }
                if ((options.ConsoleLogLevel & LogLevel.Info) == LogLevel.Info)
                {
                    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Error, consoleLog);
                    //config.AddRuleForOneLevel(NLog.LogLevel.Info, consoleLog);
                }
                if ((options.ConsoleLogLevel & LogLevel.Debug) == LogLevel.Debug)
                {
                    config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Error, consoleLog);
                    //config.AddRuleForOneLevel(NLog.LogLevel.Debug, consoleLog);
                }
            }
            NLog.LogManager.Configuration = config;
            _logger = NLog.LogManager.GetLogger("jimuLogger");
        }
        public void Info(string info)
        {
            _logger.Info(info);
        }

        public void Warn(string info)
        {
            _logger.Warn(info);
        }

        public void Error(string info, Exception ex)
        {
            _logger.Error(ex, info);
        }

        public void Debug(string info)
        {
            _logger.Debug(info);
        }
    }
}
