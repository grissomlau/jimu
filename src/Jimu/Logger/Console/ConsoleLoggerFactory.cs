using System;
using System.Runtime.CompilerServices;

namespace Jimu.Logger.Console
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type = null, [CallerFilePath] string path = "")
        {
            return new ConsoleLogger();
        }
    }
}
