using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public class DotNettyAddress : JimuAddress
    {
        public DotNettyAddress() : base("DotNetty")
        {
        }

        public DotNettyAddress(string ip, int port) : base("DotNetty")
        {
            this.Ip = ip;
            this.Port = port;

        }
    }
}
