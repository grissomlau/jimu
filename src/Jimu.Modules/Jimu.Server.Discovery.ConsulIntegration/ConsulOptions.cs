namespace Jimu.Server.Discovery.ConsulIntegration
{
    public class ConsulOptions
    {
        /// <summary>
        /// consul ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// consul port
        /// </summary>
        public int Port { get; set; }
        /// which groups to extract, multiple seperate with ','
        public string ServiceGroups { get; set; }
        /// <summary>
        /// server address, format like: ip:port, e.g.: 192.168.0.10:8080
        /// </summary>
        public string ServerAddress { get; set; }

        public ConsulOptions(string ip, int port, string serviceGroups, string serverAddress)
        {
            this.Ip = ip;
            this.Port = port;
            this.ServiceGroups = serviceGroups;
            this.ServerAddress = serverAddress;
        }

        public ConsulOptions() { }
    }
}
