namespace Jimu.Logger.Log4net
{
    public class JimuLog4netOptions
    {
        public bool EnableConsoleLog { get; set; } = true;
        public bool EnableFileLog { get; set; }
        public string FileLogPath { get; set; } = "log";
        public LogLevel FileLogLevelMax { get; set; } = LogLevel.Debug;
        public LogLevel FileLogLevelMin { get; set; } = LogLevel.Debug;
        public LogLevel ConsoleLogLevelMax { get; set; } = LogLevel.Debug;
        public LogLevel ConsoleLogLevelMin { get; set; } = LogLevel.Debug;

        public bool UseInService { get; set; }

        /// <summary>
        /// specify custom configuration file, e.g.: nlog.config
        /// </summary>
        public string Configuration { get; set; }
    }
}