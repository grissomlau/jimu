using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.Transport
{
    public class TransportOptions
    {
        /// <summary
        /// communication protocol: Netty, Http
        /// </summary>
        public string Protocol { get; set; }
    }
}
