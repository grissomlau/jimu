using System;

namespace Jimu.Logger
{
    [Flags]
    public enum LogLevel
    {
        Debug = 2,
        Info = 4,
        Warn = 8,
        Error = 16
    }
}
