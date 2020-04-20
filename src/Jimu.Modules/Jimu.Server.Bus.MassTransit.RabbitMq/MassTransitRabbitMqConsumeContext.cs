using Jimu.Core.Bus;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class MassTransitRabbitMqConsumeContext<T> : IJimuConsumeContext<T> where T : IJimuCommand
    {
        public MassTransitRabbitMqConsumeContext(IJimuBus bus, T message)
        {
            JimuBus = bus;
            Message = message;
        }

        public T Message { get; private set; }

        public IJimuBus JimuBus { get; private set; }
    }
}
