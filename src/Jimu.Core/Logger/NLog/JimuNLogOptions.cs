namespace Jimu.Logger.NLog
{
    public class JimuNLogOptions
    {
        public bool EnableConsoleLog { get; set; } = true;
        public bool EnableFileLog { get; set; }
        public string FileLogPath { get; set; } = "log";
        public LogLevel FileLogLevel { get; set; } = LogLevel.Error;
        public LogLevel ConsoleLogLevel { get; set; } = LogLevel.Debug;
        public bool UseInService { get; set; }

        /// <summary>
        /// specify custom configuration file, e.g.: nlog.config
        /// </summary>
        public string Configuration { get; set; }
    }
}
