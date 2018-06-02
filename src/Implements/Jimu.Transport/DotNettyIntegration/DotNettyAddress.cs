using System.Net;

namespace Jimu
{
    public class DotNettyAddress : JimuAddress
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
