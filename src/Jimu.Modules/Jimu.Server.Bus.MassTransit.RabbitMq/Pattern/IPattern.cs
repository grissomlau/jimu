using Autofac;
using Jimu.Core.Bus;
using Jimu.Logger;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Bus.MassTransit.RabbitMq.Pattern
{
    interface IPattern
    {
        void Register(ContainerBuilder serviceContainerBuilder);
        void MasstransitConfig(IRabbitMqBusFactoryConfigurator configurator, IContainer container, ILogger logger, IJimuBus bus, MassTransitOptions options);
    }
}
