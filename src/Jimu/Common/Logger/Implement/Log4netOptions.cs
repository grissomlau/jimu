using System;

namespace Jimu
{
    public class Log4netOptions
    {
        public bool EnableConsoleLog { get; set; }
        public bool EnableFileLog { get; set; }
        public string FileLogPath { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Error | LogLevel.Info | LogLevel.Warn;

    }

    [Flags]
    public enum LogLevel
    {
        Error = 2,
        Warn = 4,
        Info = 8,
    }
}
