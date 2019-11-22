using System;

namespace Jimu
{
    public class TransportException : Exception
    {
        public TransportException(string msg, Exception innerException) : base(msg, innerException)
        {
        }

    }
}
