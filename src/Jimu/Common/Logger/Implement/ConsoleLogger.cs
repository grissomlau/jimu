using System;

namespace Jimu.Common.Logger
{
    /// <summary>
    ///     echo log throgh console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        public void Debug(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {info}");
        }

        public void Error(string info, Exception ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {info},{ex.ToStackTraceString()}");
        }

        public void Info(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {info}");
        }

        public void Warn(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss.fff")} {info}"); ;
        }
    }
}