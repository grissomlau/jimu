using Jimu.Extension;
using System;

namespace Jimu.Logger
{
    /// <summary>
    ///     echo log throgh console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        string _ip;
        public ConsoleLogger()
        {

            _ip = JimuHelper.GetLocalIPAddress();
        }
        public void Debug(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} DEBUG [{_ip}] {info}");
        }

        public void Error(string info, Exception ex)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} ERROR [{_ip}] {info},{ex.ToStackTraceString()}");
        }

        public void Info(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} INFO [{_ip}] {info}");
        }

        public void Warn(string info)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} WARN [{_ip}] {info}");
        }
    }
}