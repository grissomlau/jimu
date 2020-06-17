using System;
using System.Runtime.CompilerServices;

namespace Jimu.Logger
{
    public interface ILoggerFactory
    {
        ILogger Create(Type type = null, [CallerFilePath] string path = "");
    }
}
