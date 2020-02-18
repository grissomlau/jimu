namespace Jimu.Client.Discovery.Consul
{
    public class ConsulOptions
    {
        public bool Enable { get; set; } = true;
        /// <summary>
        /// consul ip
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// consul port
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// which groups to extract, multiple seperate with ','
        /// </summary>
        public string ServiceGroups { get; set; }


        public ConsulOptions(string ip, string port, string serviceGroups)
        {
            this.Ip = ip;
            this.Port = port;
            this.ServiceGroups = serviceGroups;
        }

        public ConsulOptions() { }
    }
}
