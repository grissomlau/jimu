
namespace Jimu
{
    public class NetCoreHttpAddress : JimuAddress
    {
        public NetCoreHttpAddress(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
