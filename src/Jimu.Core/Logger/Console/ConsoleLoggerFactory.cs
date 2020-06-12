using Jimu.Logger;
using Jimu.Logger.Console;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jimu.Core.Logger.Console
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public ILogger Create(Type type = null, [CallerFilePath] string path = "")
        {
            return new ConsoleLogger();
        }
    }
}
