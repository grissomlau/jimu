namespace Jimu.Client.Discovery.ConsulIntegration
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
        /// <summary>
        /// which groups to extract, multiple seperate with ','
        /// </summary>
        public string ServiceGroups { get; set; }


        public ConsulOptions(string ip, int port, string serviceGroups)
        {
            this.Ip = ip;
            this.Port = port;
            this.ServiceGroups = serviceGroups;
        }

        public ConsulOptions() { }
    }
}
