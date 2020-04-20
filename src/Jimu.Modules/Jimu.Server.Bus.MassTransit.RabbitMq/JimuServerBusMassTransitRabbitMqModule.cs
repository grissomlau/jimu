using Autofac;
using Jimu.Core.Bus;
using Jimu.Logger;
using Jimu.Server.ServiceContainer.Implement.Parser;
using Jimu.Server.Transport;
using MassTransit;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MT = MassTransit;

namespace Jimu.Server.Bus.MassTransit.RabbitMq
{
    public class JimuServerBusMassTransitRabbitMqModule : ServerGeneralModuleBase
    {
        readonly MassTransitOptions _options;
        List<Type> _consumers = new List<Type>();
        List<Type> _subscribers = new List<Type>();
        IJimuBus _bus = null;
        IBusControl _massTransitBus = null;
        public JimuServerBusMassTransitRabbitMqModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(MassTransitOptions).Name).Get<MassTransitOptions>();
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null && _options.Enable)
            {

                //register command, because we need get the QueueName from it's instance when send and consume it
                var commands = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuCommand") != null)
                    .ToList();
                if (commands.Any())
                {
                    serviceContainerBuilder.RegisterTypes(commands.ToArray()).AsSelf().InstancePerDependency();
                }
                // register consumer
                _consumers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuConsumer`1") != null
                && x.GetInterface("IJimuConsumer`1").GetGenericTypeDefinition() == typeof(IJimuConsumer<>))
                .ToList();
                if (_consumers.Any())
                {
                    serviceContainerBuilder.RegisterTypes(_consumers.ToArray()).AsSelf().InstancePerDependency();
                }

                // register subscriber
                var subscriberType = typeof(IJimuSubscriber<>);
                _subscribers = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuSubscriber`1") != null
                && x.GetInterface("IJimuSubscriber`1").GetGenericTypeDefinition() == typeof(IJimuSubscriber<>))
                .ToList();

                if (_subscribers.Any())
                {
                    serviceContainerBuilder.RegisterTypes(_subscribers.ToArray()).AsSelf().InstancePerDependency();
                }

                serviceContainerBuilder.Register(x => _bus);
                serviceContainerBuilder.Register<IBus>(x => _massTransitBus);
            }

            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override async void DoServiceInit(IContainer container)
        {
            if (_options != null && _options.Enable)
            {
                var logger = container.Resolve<ILogger>();
                _massTransitBus = MT.Bus.Factory.CreateUsingRabbitMq(sbc =>
               {
                   var host = sbc.Host($"rabbitmq://{_options.HostAddress}", h =>
                   {
                       if (!string.IsNullOrEmpty(_options.UserName))
                       {
                           h.Username(_options.UserName);
                           h.Password(_options.Password);
                       }
                   });

                   if (_subscribers.Any())
                   {
                       //-- event subscriber, generate default event queue, if not specify in options

                       var eventQueueName = string.IsNullOrEmpty(_options.EventQueueName) ? $"{AppDomain.CurrentDomain.FriendlyName}-{Guid.NewGuid()}" : _options.EventQueueName;


                       sbc.ReceiveEndpoint(eventQueueName, ep =>
                       {
                           _subscribers.ForEach(subscriber =>
                           {
                               var subscriberType = subscriber.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                               var handlerMethod = typeof(HandlerExtensions).GetMethod("Handler").MakeGenericMethod(subscriberType);
                               var subscriberInstance = container.Resolve(subscriber);
                               var myHandlerObj = Activator.CreateInstance(typeof(EventHandler<>).MakeGenericType(subscriberType), new object[] { subscriberInstance, _bus });
                               var eventHandler = myHandlerObj.GetType().InvokeMember("Handler", BindingFlags.GetProperty, null, myHandlerObj, null);
                               var fastInvoker = FastInvoke.GetMethodInvoker(handlerMethod);
                               fastInvoker.Invoke(null, new object[] { ep, eventHandler, null });
                           });
                       });
                       logger.Debug($"MassTransitRabbitMq: EventQueueName: { eventQueueName}, subscribers count: {_subscribers.Count()}");
                   }


                   //-- command consumer, extract queue name from command
                   var groupConsumers = _consumers.GroupBy(x =>
                   {
                       var commandType = x.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                       var commandInstance = container.Resolve(commandType);
                       if (commandInstance == null) throw new Exception($"JimuCommand: {commandType.FullName} resolve failure");
                       var commandQueueName = commandInstance.GetType().InvokeMember("QueueName", BindingFlags.GetProperty, null, commandInstance, null) + "";
                       if (string.IsNullOrEmpty(commandQueueName))
                       {
                           throw new Exception($"JimuCommand: {commandType.FullName} must specify QueueName property");
                       }
                       return commandQueueName;
                   });
                   groupConsumers.ToList().ForEach(x =>
                   {
                       sbc.ReceiveEndpoint(x.Key, ep =>
                       {
                           logger.Debug($"MassTransitRabbitMq: CommandQueueName: { x.Key}, consumers count: {x.Count()}");
                           x.ToList().ForEach(consumer =>
                           {
                               var commandType = consumer.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                               var handlerMethod = typeof(HandlerExtensions).GetMethod("Handler").MakeGenericMethod(commandType);
                               var consumerInstance = container.Resolve(consumer);
                               var myHandlerObj = Activator.CreateInstance(typeof(CommandHandler<>).MakeGenericType(commandType), new object[] { consumerInstance, _bus });
                               var consumerHandler = myHandlerObj.GetType().InvokeMember("Handler", BindingFlags.GetProperty, null, myHandlerObj, null);
                               var fastInvoker = FastInvoke.GetMethodInvoker(handlerMethod);
                               fastInvoker.Invoke(null, new object[] { ep, consumerHandler, null });
                           });
                       });
                   });

               });

                _bus = new MassTransitRabbitMqBus(_massTransitBus);


                try
                {
                    await _massTransitBus.StartAsync(); // This is important!

                    logger.Info($"MassTransitRabbitMq start successfully: rabbitmq://{_options.HostAddress}, username: {_options.UserName}, password: {_options.Password}");
                }
                catch (Exception ex)
                {
                    logger.Error($"MassTransitRabbitMq start failed: rabbitmq://{_options.HostAddress}, username: {_options.UserName}, password: {_options.Password}", ex);
                }

            }
            base.DoServiceInit(container);
        }

        public override void DoDispose(IContainer container)
        {
            if (_massTransitBus != null)
            {
                try
                {
                    _massTransitBus.Stop();
                }
                catch (Exception ex)
                {
                }
            }

            base.DoDispose(container);
        }


        public class CommandHandler<T> where T : class, IJimuCommand
        {
            IJimuConsumer<T> _consumer;
            IJimuBus _bus;
            public CommandHandler(IJimuConsumer<T> consumer, IJimuBus bus)
            {
                _consumer = consumer;
                _bus = bus;
            }
            public MessageHandler<T> Handler => context => _consumer.ConsumeAsync(new MassTransitRabbitMqConsumeContext<T>(_bus, context.Message));
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
