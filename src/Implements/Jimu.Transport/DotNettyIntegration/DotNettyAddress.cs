using System.Net;
using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.DotNettyIntegration
{
    public class DotNettyAddress : Address
    {
        public DotNettyAddress() { }
        public DotNettyAddress(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }

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
