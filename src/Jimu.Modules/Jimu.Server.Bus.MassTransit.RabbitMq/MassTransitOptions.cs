using System;
using System.Collections.Generic;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitOptions
    {
        /// <summary>
        /// rabbitmq host
        /// </summary>
        public string HostAddress { get; set; }
        /// <summary>
        /// rabbitmq username
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// rabbitmq password
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// queue for event 
        /// </summary>
        public string EventQueueName { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; } = true;
    }

}
