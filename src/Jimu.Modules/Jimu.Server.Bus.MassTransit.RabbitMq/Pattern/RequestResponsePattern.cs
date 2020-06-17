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
    internal class RequestResponsePattern : IPattern
    {
        List<Type> _requestHandlers = new List<Type>();
        public void MasstransitConfig(IRabbitMqBusFactoryConfigurator configurator, IContainer container, ILogger logger, IJimuBus bus, MassTransitOptions options)
        {
            //-- request handler, extract queue name from request
            var groupHandlers = _requestHandlers.GroupBy(x =>
            {
                var requestType = x.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                var requestInstance = container.Resolve(requestType);
                if (requestInstance == null) throw new Exception($"JimuRequest: {requestType.FullName} resolve failure");
                var requestQueueName = requestInstance.GetType().InvokeMember("QueueName", BindingFlags.GetProperty, null, requestInstance, null) + "";
                if (string.IsNullOrWhiteSpace(requestQueueName))
                {
                    throw new Exception($"JimuRequest: {requestType.FullName} must specify QueueName property");
                }
                return requestQueueName;
            });


            groupHandlers.ToList().ForEach(x =>
            {
                configurator.ReceiveEndpoint(x.Key, ep =>
                {
                    logger.Debug($"MassTransitRabbitMq: RequestQueueName: { x.Key}, handler count: {x.Count()}");
                    x.ToList().ForEach(handler =>
                    {
                        var requestType = handler.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments.First();
                        var respType = handler.GetTypeInfo().ImplementedInterfaces.First().GenericTypeArguments[1];
                        var handlerMethod = typeof(HandlerExtensions).GetMethod("Handler").MakeGenericMethod(requestType);
                        var requestInstance = container.Resolve(handler);
                        var myHandlerObj = Activator.CreateInstance(typeof(RequestHandler<,>).MakeGenericType(requestType, respType), new object[] { requestInstance, bus });
                        var requestHandler = myHandlerObj.GetType().InvokeMember("Handler", BindingFlags.GetProperty, null, myHandlerObj, null);
                        var fastInvoker = FastInvoke.GetMethodInvoker(handlerMethod);
                        fastInvoker.Invoke(null, new object[] { ep, requestHandler, null });
                    });
                });
            });
        }

        public void Register(ContainerBuilder serviceContainerBuilder)
        {
            //register request, because we need get the QueueName from it's instance when send and handle it
            var requests = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuRequest") != null)
                .ToList();
            if (requests.Any())
            {
                serviceContainerBuilder.RegisterTypes(requests.ToArray()).AsSelf().InstancePerDependency();
            }

            // register requestHandler
            _requestHandlers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.GetInterface("IJimuRequestHandler`2") != null
            && x.GetInterface("IJimuRequestHandler`2").GetGenericTypeDefinition() == typeof(IJimuRequestHandler<,>))
            .ToList();

            if (_requestHandlers.Any())
            {
                serviceContainerBuilder.RegisterTypes(_requestHandlers.ToArray()).AsSelf().AsImplementedInterfaces().InstancePerDependency();
            }
        }

        public class RequestHandler<Req, Resp> where Req : class, IJimuRequest where Resp : class
        {
            IJimuRequestHandler<Req, Resp> _handler;
            IJimuBus _bus;
            public RequestHandler(IJimuRequestHandler<Req, Resp> handler, IJimuBus bus)
            {
                _handler = handler;
                _bus = bus;
            }
            public MessageHandler<Req> Handler => async context =>
           {
               var resp = await _handler.HandleAsync(new MassTransitRabbitMqRequestContext<Req>(_bus, context.Message));
               await context.RespondAsync(resp);
           };
        }
    }
}
