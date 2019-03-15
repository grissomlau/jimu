namespace Jimu.Server.Transport.DotNetty
{
    public class DotNettyOptions
    {
        public string Ip { get; set; }
        public int Port { get; set; }

        public DotNettyOptions(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        public DotNettyOptions() { }

    }
}
