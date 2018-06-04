using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public class TransportException : Exception
    {
        public TransportException(string msg, Exception innerException) : base(msg, innerException)
        {
        }

    }
}
