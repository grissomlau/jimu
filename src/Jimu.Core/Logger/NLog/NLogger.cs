using Jimu.Common;
using System;
using N = NLog;

namespace Jimu.Logger.NLog
{
    public class NLogger : ILogger
    {
        private readonly N.Logger _logger;

        public NLogger(JimuNLogOptions options = null)
        {
            options = options ?? new JimuNLogOptions { EnableConsoleLog = true };
            if (!string.IsNullOrEmpty(options.Configuration))
            {
                N.LogManager.Configuration = new N.Config.XmlLoggingConfiguration("nlog.config");
            }
            else
            {
                var config = new N.Config.LoggingConfiguration();
                var ip = JimuHelper.GetLocalIPAddress();
                if (options.EnableFileLog)
                {
                    var fileConf = new N.Targets.FileTarget("jimuLogFile")
                    {
                        FileName = "./log/${level:lowercase=true}/${shortdate}.log",
                        ArchiveAboveSize = 10000000,
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [" + ip + "] ${message}"
                    };
                    if (options.FileLogPath != null)
                    {
                        fileConf.FileName = options.FileLogPath + "/${level:lowercase=true}/${shortdate}.log";

                    }
                    if ((options.FileLogLevel & LogLevel.Error) == LogLevel.Error)
                    {
                        config.AddRuleForOneLevel(N.LogLevel.Error, fileConf);
                    }
                    if ((options.FileLogLevel & LogLevel.Warn) == LogLevel.Warn)
                    {
                        config.AddRule(N.LogLevel.Warn, N.LogLevel.Error, fileConf);
                    }
                    if ((options.FileLogLevel & LogLevel.Info) == LogLevel.Info)
                    {
                        config.AddRule(N.LogLevel.Info, N.LogLevel.Error, fileConf);
                    }
                    if ((options.FileLogLevel & LogLevel.Debug) == LogLevel.Debug)
                    {
                        config.AddRule(N.LogLevel.Debug, N.LogLevel.Error, fileConf);
                    }
                }
                if (options.EnableConsoleLog)
                {
                    var consoleLog = new N.Targets.ConsoleTarget("jimuLogconsole")
                    {
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [" + ip + "] ${message}"
                    };
                    if ((options.ConsoleLogLevel & LogLevel.Error) == LogLevel.Error)
                    {
                        config.AddRuleForOneLevel(N.LogLevel.Error, consoleLog);
                    }
                    if ((options.ConsoleLogLevel & LogLevel.Warn) == LogLevel.Warn)
                    {
                        config.AddRule(N.LogLevel.Warn, N.LogLevel.Error, consoleLog);
                    }
                    if ((options.ConsoleLogLevel & LogLevel.Info) == LogLevel.Info)
                    {
                        config.AddRule(N.LogLevel.Info, N.LogLevel.Error, consoleLog);
                    }
                    if ((options.ConsoleLogLevel & LogLevel.Debug) == LogLevel.Debug)
                    {
                        config.AddRule(N.LogLevel.Debug, N.LogLevel.Error, consoleLog);
                    }
                }
                N.LogManager.Configuration = config;
            }
            _logger = N.LogManager.GetLogger("*",typeof(N.LogManager));
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
