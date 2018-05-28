using Jimu.Core.Protocols;

namespace Jimu.Common.Transport.NetCoreHttpIntegration
{
    public class NetCoreHttpAddress : Address
    {
        public NetCoreHttpAddress(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
