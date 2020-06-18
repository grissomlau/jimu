using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
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
                        ArchiveAboveSize = 10000000,
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [${threadid}] ${message}"
                    };
                    string path = string.IsNullOrEmpty(options.FileLogPath) ? "./log" : options.FileLogPath;
                    fileConf.FileName = path + "/${shortdate}.log";
                    config.AddRule(ConvertToLevel(options.FileLogLevelMin), ConvertToLevel(options.FileLogLevelMax), fileConf);
                }
                if (options.EnableConsoleLog)
                {
                    var consoleLog = new N.Targets.ConsoleTarget("jimuLogconsole")
                    {
                        Layout = @"${date:format=yyyy-MM-dd HH\:mm\:ss.fff} ${level:uppercase=true} [${threadid}] ${message}"
                    };
                    config.AddRule(ConvertToLevel(options.ConsoleLogLevelMin), ConvertToLevel(options.ConsoleLogLevelMax), consoleLog);
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

        private static N.LogLevel ConvertToLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return N.LogLevel.Debug;
                case LogLevel.Info:
                    return N.LogLevel.Info;
                case LogLevel.Warn:
                    return N.LogLevel.Warn;
                case LogLevel.Error:
                    return N.LogLevel.Error;
                default:
                    throw new LoggerException($"{logLevel} not found int NLog");
            }
        }
    }
}
