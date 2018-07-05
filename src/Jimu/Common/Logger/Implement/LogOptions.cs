namespace Jimu
{
    public class LogOptions
    {
        public bool EnableConsoleLog { get; set; }
        public bool EnableFileLog { get; set; }
        public string FileLogPath { get; set; } = "logs";
        public LogLevel FileLogLevel { get; set; } = LogLevel.Error;
        public LogLevel ConsoleLogLevel { get; set; } = LogLevel.Error | LogLevel.Info | LogLevel.Warn;

    }
}