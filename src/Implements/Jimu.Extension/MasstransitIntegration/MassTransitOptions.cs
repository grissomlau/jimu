using System;

namespace Jimu.Server
{
    public class MassTransitOptions
    {
        public Uri SendEndPointUri { get; set; }
        public Uri HostAddress { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string QueueName { get; set; }
    }
}
