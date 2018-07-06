using System;

namespace Jimu
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
