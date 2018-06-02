namespace Jimu.Common.Logger.Log4netIntegration
{
    public class Log4netOptions
    {
        public bool EnableConsoleLog { get; set; }
        public bool EnableFileLog { get; set; }
        public string FileLogPath { get; set; }
        public LogLevel LogLevel { get; set; }

    }

    public enum LogLevel
    {
        Error = 2,
        Info = 4
    }
}
