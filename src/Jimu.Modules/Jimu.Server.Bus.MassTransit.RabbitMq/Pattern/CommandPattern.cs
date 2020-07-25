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
    internal class CommandPattern : IPattern
    {
        List<Type> _consumers = new List<Type>();


        public void MasstransitConfig(IRabbitMqBusFactoryConfigurator configurator, IContainer container, ILogger logger, IJimuBus bus, MassTransitOptions options)
        {
            //-- command consumer, extract queue name from command
            var groupConsumers = _consumers.GroupBy(x =>
            {
                var commandType = x.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                var commandInstance = container.Resolve(commandType);
                if (commandInstance == null) throw new Exception($"JimuCommand: {commandType.FullName} resolve failure");
                var commandQueueName = commandInstance.GetType().InvokeMember("QueueName", BindingFlags.GetProperty, null, commandInstance, null) + "";
                if (string.IsNullOrWhiteSpace(commandQueueName))
                {
                    throw new Exception($"JimuCommand: {commandType.FullName} must specify QueueName property");
                }
                return commandQueueName;
            });
            groupConsumers.ToList().ForEach(x =>
            {
                configurator.ReceiveEndpoint(x.Key, ep =>
                {
                    logger.Debug($"MassTransitRabbitMq: CommandQueueName: { x.Key}, consumers count: {x.Count()}");
                    x.ToList().ForEach(consumer =>
                    {
                        var commandType = consumer.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                        var handlerMethod = typeof(HandlerExtensions).GetMethod("Handler").MakeGenericMethod(commandType);
                        var consumerInstance = container.Resolve(consumer);
                        var myHandlerObj = Activator.CreateInstance(typeof(CommandHandler<>).MakeGenericType(commandType), new object[] { consumerInstance, bus });
                        var consumerHandler = myHandlerObj.GetType().InvokeMember("Handler", BindingFlags.GetProperty, null, myHandlerObj, null);
                        var fastInvoker = FastInvoke.GetMethodInvoker(handlerMethod);
                        try
                        {
                            fastInvoker.Invoke(null, new object[] { ep, consumerHandler, null });
                        }
                        catch (Exception ex)
                        {
                            logger.Error($"error occure when handling RabbitMq consumer: {x.Key}", ex);
                        }
                    });
                });
            });
        }

        public void Register(ContainerBuilder serviceContainerBuilder)
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
                serviceContainerBuilder.RegisterTypes(_consumers.ToArray()).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }
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
    }
}
