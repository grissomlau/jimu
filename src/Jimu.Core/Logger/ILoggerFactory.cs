using Jimu.Logger;
using System;
using System.Runtime.CompilerServices;

namespace Jimu
{
    public interface ILoggerFactory
    {
        ILogger Create(Type type = null, [CallerFilePath] string path = "");
    }
}
