using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Jimu
{
    /// <summary>
    /// 基于 ip 和 port 的服务器地址
    /// </summary>
    public class IpAddress : Address
    {
        public IpAddress(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }
        public string Ip { get; set; }

        public int Port { get; set; }
        public override EndPoint CreateEndPoint()
        {
            return new IPEndPoint(IPAddress.Parse(Ip), Port);
        }

        public override string ToString()
        {
            return $"{Ip}:{Port}";
        }
    }
}
