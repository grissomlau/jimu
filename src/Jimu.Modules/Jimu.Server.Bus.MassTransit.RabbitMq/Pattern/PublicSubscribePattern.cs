using Autofac;
using Jimu.Bus;
using Jimu.Logger;
using Jimu.Server.ServiceContainer.Implement.Parser;
using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jimu.Server.Bus.MassTransit.RabbitMq.Pattern
{
    internal class PublicSubscribePattern : IPattern
    {
        List<Type> _subscribers = new List<Type>();
        public void MasstransitConfig(IRabbitMqBusFactoryConfigurator configurator, IContainer container, ILogger logger, IJimuBus bus, MassTransitOptions _options)
        {
            if (_subscribers.Any())
            {
                //-- event subscriber, generate default event queue, if not specify in options

                var eventQueueName = string.IsNullOrEmpty(_options.EventQueueName) ? $"{AppDomain.CurrentDomain.FriendlyName}-{Guid.NewGuid()}" : _options.EventQueueName;

                configurator.ReceiveEndpoint(eventQueueName, ep =>
                {
                    _subscribers.ForEach(subscriber =>
                    {
                        var subscriberType = subscriber.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                        var handlerMethod = typeof(HandlerExtensions).GetMethod("Handler").MakeGenericMethod(subscriberType);
                        var subscriberInstance = container.Resolve(subscriber);
                        var myHandlerObj = Activator.CreateInstance(typeof(EventHandler<>).MakeGenericType(subscriberType), new object[] { subscriberInstance, bus });
                        var eventHandler = myHandlerObj.GetType().InvokeMember("Handler", BindingFlags.GetProperty, null, myHandlerObj, null);
                        var fastInvoker = FastInvoke.GetMethodInvoker(handlerMethod);
                        try
                        {
                            fastInvoker.Invoke(null, new object[] { ep, eventHandler, null });
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"error occure when handling RabbitMq subscriber: {eventQueueName}", ex);
                        }
                    });
                });
                logger.Debug($"MassTransitRabbitMq: EventQueueName: { eventQueueName}, subscribers count: {_subscribers.Count()}");
            }
        }

        public void Register(ContainerBuilder serviceContainerBuilder)
        {
            // register subscriber
            _subscribers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuSubscriber`1") != null
            && x.GetInterface("IJimuSubscriber`1").GetGenericTypeDefinition() == typeof(IJimuSubscriber<>))
            .ToList();

            if (_subscribers.Any())
            {
                serviceContainerBuilder.RegisterTypes(_subscribers.ToArray()).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }
        }

        public class EventHandler<T> where T : class, IJimuEvent
        {
            IJimuSubscriber<T> _subscriber;
            IJimuBus _bus;
            public EventHandler(IJimuSubscriber<T> consumer, IJimuBus bus)
            {
                _subscriber = consumer;
                _bus = bus;
            }
            public MessageHandler<T> Handler => context => _subscriber.HandleAsync(new MassTransitRabbitMqSubscribeContext<T>(_bus, context.Message));
        }
    }
}
