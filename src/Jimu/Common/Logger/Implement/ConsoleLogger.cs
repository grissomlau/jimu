using System;

namespace Jimu.Common.Logger
{
    /// <summary>
    ///     echo log throgh console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Error(string info, Exception ex)
        {
            Console.WriteLine($"{info},{ex.ToStackTraceString()}");
        }

        public void Info(string info)
        {
            Console.WriteLine($"{info}");
        }

        public void Warn(string info)
        {
            Console.WriteLine($"{info}"); ;
        }
    }
}