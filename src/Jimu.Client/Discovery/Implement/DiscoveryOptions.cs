using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.Discovery.Implement
{
    public class DiscoveryOptions
    {
        public int UpdateJobIntervalMinute { get; set; }
        public DiscoveryOptions(int updateJobIntervalMinute)
        {
            this.UpdateJobIntervalMinute = updateJobIntervalMinute;
        }
        public DiscoveryOptions() { }
    }
}
