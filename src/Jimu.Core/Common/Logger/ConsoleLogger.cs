using System;

namespace Jimu
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
    }
}