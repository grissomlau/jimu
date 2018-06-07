using System;
using Autofac;
using MassTransit;
using MassTransit.RabbitMqTransport;

namespace Jimu.Server
{
    public static class ServiceHostBuilderExtension
    {
        public static IServiceHostServerBuilder UseMasstransit(this IServiceHostServerBuilder serviceHostBuilder, MassTransitOptions options, Action<IRabbitMqBusFactoryConfigurator> action = null)
        {
            serviceHostBuilder.RegisterService(containerBuilder =>
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
                var bus = container.Resolve<IBusControl>();
                bus.StartAsync();
                IServiceHost host = container.Resolve<IServiceHost>();
                host.DisposeAction(c =>
                {
                    bus.Stop();
                });
            });
            return serviceHostBuilder;
        }
    }
}
