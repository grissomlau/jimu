using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Client.LoadBalance
{
    public class LoadBalanceOptions
    {
        /// <summary>
        /// 负载算法，已实现： Polling
        /// </summary>
        public string LoadBalance { get; set; }
    }
}
