using Jimu.Logger;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jimu.Common
{
    public class JimuOptions
    {
        public int MaxRequestContentLengthMB { get; set; } = 100;
        public LogLevel logLevel { get; set; } = LogLevel.Info;

        private static JimuOptions _instance;
        private static object locker = new object();
        public static JimuOptions Get(IConfigurationRoot settings)
        {
            lock (locker)
            {
                if (_instance != null)
                {
                    return _instance;
                }
                _instance = settings.GetSection(typeof(JimuOptions).Name).Get<JimuOptions>();
                return _instance == null ? new JimuOptions() : _instance;
            }
        }
        public JimuOptions()
        {
        }

    }
}
