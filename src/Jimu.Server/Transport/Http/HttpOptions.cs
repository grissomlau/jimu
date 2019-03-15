namespace Jimu.Server.Transport.Http
{
    public class HttpOptions
    {
        public string Ip { get; set; }
        public int Port { get; set; }

        public HttpOptions(string ip, int port)
        {
            this.Ip = ip;
            this.Port = port;
        }

        public HttpOptions() { }
    }
}
