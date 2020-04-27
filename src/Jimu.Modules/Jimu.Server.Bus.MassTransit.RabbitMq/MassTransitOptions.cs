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
        /// whether enable 
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// timeout for request method(seconds),default 30 seconds
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;
    }

}
