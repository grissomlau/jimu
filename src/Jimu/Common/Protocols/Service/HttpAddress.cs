using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public class HttpAddress : JimuAddress
    {
        public HttpAddress() : base("Http")
        {
        }

        public HttpAddress(string ip, int port) : base("Http")
        {
            this.Ip = ip;
            this.Port = port;
        }
    }
}
