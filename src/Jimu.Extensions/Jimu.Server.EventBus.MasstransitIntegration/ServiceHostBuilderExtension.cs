using System;
using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Jimu.Server
{
    public static class ServiceHostBuilderExtension
    {
        public static IApplicationServerBuilder UseMasstransit(this IApplicationServerBuilder serviceHostBuilder, MassTransitOptions options, Action<IRabbitMqBusFactoryConfigurator> action = null)
        {
            serviceHostBuilder.RegisterComponent(containerBuilder =>
            {
                containerBuilder.Register(x => options).SingleInstance();
                containerBuilder.Register(context =>
                {
                    var bc = Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(options.HostAddress, h =>
                        {
                            h.Username(options.Username);
                            h.Password(options.Password);
                        });

                        cfg.ReceiveEndpoint(options.QueueName, ec =>
                        {
                            ec.LoadFrom(context);
                        });

                        action?.Invoke(cfg);
                    });
                    return bc;
                }).SingleInstance().As<IBusControl>().As<IBus>();
            });

            serviceHostBuilder.AddInitializer(container =>
            {
                var logger = container.Resolve<ILogger>();
                logger.Info($"[config]use Masstransit for EventBus, options.HostAddress {options.HostAddress.ToString()}, options.SendEndPointUrl {options.SendEndPointUri.ToString()}");
                var bus = container.Resolve<IBusControl>();
                bus.StartAsync();
                IApplication host = container.Resolve<IApplication>();
                host.DisposeAction(c =>
                {
                    bus.Stop();
                });
            });
            return serviceHostBuilder;
        }
    }
}
