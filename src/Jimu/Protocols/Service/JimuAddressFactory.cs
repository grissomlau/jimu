using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Protocols.Service
{
    public class JimuAddressFactory
    {
        /// <summary>
        /// create address by protocol
        /// </summary>
        public static event Func<string, JimuAddress> CreateAddress;


    }
}
