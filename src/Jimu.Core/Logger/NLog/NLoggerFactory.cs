using Jimu.Common;
using Jimu.Logger;
using Jimu.Logger.NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using N = NLog;

namespace Jimu.Logger.NLog
{
    public class NLoggerFactory : ILoggerFactory
    {
        public NLoggerFactory(JimuNLogOptions options = null)
        {
            options = options ?? new JimuNLogOptions { EnableConsoleLog = true };
            if (!string.IsNullOrEmpty(options.Configuration))
            {
                N.LogManager.Configuration = new N.Config.XmlLoggingConfiguration(options.Configuration);
            }
            else
            {
                var config = new N.Config.LoggingConfiguration();
                //var ip = JimuHelper.GetLocalIPAddress();
                if (options.EnableFileLog)
                {
                    var fileConf = new N.Targets.FileTarget("jimuLogFile")
                    {
                        FileName = "./log/${level:lowercase=true}/${shortdate}.log",
                        ArchiveAboveSize = 10000000,
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [${threadid}] ${message}"
                    };
                    if (options.FileLogPath != null)
                    {
                        fileConf.FileName = options.FileLogPath + "/${level:lowercase=true}/${shortdate}.log";

                    }
                    if ((options.FileLogLevel & LogLevel.Debug) == LogLevel.Debug)
                    {
                        config.AddRule(N.LogLevel.Debug, N.LogLevel.Error, fileConf);
                    }
                    else if ((options.FileLogLevel & LogLevel.Info) == LogLevel.Info)
                    {
                        config.AddRule(N.LogLevel.Info, N.LogLevel.Error, fileConf);
                    }
                    else if ((options.FileLogLevel & LogLevel.Warn) == LogLevel.Warn)
                    {
                        config.AddRule(N.LogLevel.Warn, N.LogLevel.Error, fileConf);
                    }
                    else if ((options.FileLogLevel & LogLevel.Error) == LogLevel.Error)
                    {
                        config.AddRuleForOneLevel(N.LogLevel.Error, fileConf);
                    }
                }
                if (options.EnableConsoleLog)
                {
                    var consoleLog = new N.Targets.ConsoleTarget("jimuLogconsole")
                    {
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [${threadid}] ${message}"
                    };
                    if ((options.ConsoleLogLevel & LogLevel.Debug) == LogLevel.Debug)
                    {
                        config.AddRule(N.LogLevel.Debug, N.LogLevel.Error, consoleLog);
                    }
                    else if ((options.ConsoleLogLevel & LogLevel.Info) == LogLevel.Info)
                    {
                        config.AddRule(N.LogLevel.Info, N.LogLevel.Error, consoleLog);
                    }
                    else if ((options.ConsoleLogLevel & LogLevel.Warn) == LogLevel.Warn)
                    {
                        config.AddRule(N.LogLevel.Warn, N.LogLevel.Error, consoleLog);
                    }
                    else if ((options.ConsoleLogLevel & LogLevel.Error) == LogLevel.Error)
                    {
                        config.AddRuleForOneLevel(N.LogLevel.Error, consoleLog);
                    }
                }
                N.LogManager.Configuration = config;
            }
        }
        public ILogger Create(Type type = null, [CallerFilePath] string path = "")
        {
            //_logger = N.LogManager.GetLogger("*", typeof(N.LogManager));
            var logger = N.LogManager.GetLogger("*");
            if (type == null)
            {
                string name = Path.GetFileNameWithoutExtension(path);
                return new NLogger(logger, name);
            }
            else
            {
                return new NLogger(logger, type);
            }
        }
    }
}
