using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Logger
{
    public class LoggerException : Exception
    {
        public LoggerException() { }
        public LoggerException(string msg) : base(msg) { }
        public LoggerException(string msg, Exception innerException) : base(msg, innerException)
        {

        }
    }
}
