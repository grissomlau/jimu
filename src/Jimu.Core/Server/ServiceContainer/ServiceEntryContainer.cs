using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Jimu.Core.Protocols;
using Jimu.Core.Protocols.Attributes;

namespace Jimu.Core.Server.ServiceContainer
{
    public class ServiceEntryContainer : IServiceEntryContainer
    {
        private readonly IContainer _container;
        private readonly IServiceIdGenerator _serviceIdGenerate;
        private readonly ITypeConvertProvider _typeConvertProvider;
        private readonly ConcurrentDictionary<Tuple<Type, string>, FastInvoke.FastInvokeHandler> _handler;
        private readonly List<ServiceEntry> _services;

        public ServiceEntryContainer(IContainer container, IServiceIdGenerator serviceIdGenerate,
                ITypeConvertProvider typeConvertProvider)
        //public ServiceEntryContainer()
        {
            _serviceIdGenerate = serviceIdGenerate;
            _container = container;
            _typeConvertProvider = typeConvertProvider;
            _services = new List<ServiceEntry>();
            _handler = new ConcurrentDictionary<Tuple<Type, string>, FastInvoke.FastInvokeHandler>();
            new ConcurrentDictionary<Tuple<Type, string>, object>();
        }


        public IServiceEntryContainer AddServices(Type[] types)
        {
            var serviceTypes = types.Where(x =>
            {
                var typeinfo = x.GetTypeInfo();
                return typeinfo.IsInterface && typeinfo.GetCustomAttribute<ServiceRouteAttribute>() != null;
            }).Distinct();

            foreach (var type in serviceTypes)
            {
                var routeTemplate = type.GetCustomAttribute<ServiceRouteAttribute>();
                foreach (var methodInfo in type.GetTypeInfo().GetMethods())
                {
                    var serviceId = _serviceIdGenerate.GenerateServiceId(methodInfo);
                    var fastInvoker = GetHandler(serviceId, methodInfo);
                    var service = new ServiceEntry
                    {
                        Descriptor = new ServiceDescriptor
                        {
                            Id = serviceId,
                            RoutePath = ServiceRoute.ParseRoutePath(routeTemplate.RouteTemplate, type.Name,
                                methodInfo.Name, methodInfo.GetParameters())
                        },
                        Func = (paras, payload) =>
                        {
                            var instance = GetInstance(null, methodInfo.DeclaringType, payload);
                            var parameters = new List<object>();
                            foreach (var para in methodInfo.GetParameters())
                            {
                                paras.TryGetValue(para.Name, out var value);
                                var paraType = para.ParameterType;
                                var parameter = _typeConvertProvider.Convert(value, paraType);
                                parameters.Add(parameter);
                            }

                            var result = fastInvoker(instance, parameters.ToArray());
                            return Task.FromResult(result);
                        }
                    };
                    var descriptorAttributes = methodInfo.GetCustomAttributes<ServiceDescriptorAttribute>();
                    foreach (var attr in descriptorAttributes) attr.Apply(service.Descriptor);
                    if (methodInfo.ReturnType.ToString().IndexOf("System.Threading.Tasks.Task", StringComparison.Ordinal) == 0 &&
                        methodInfo.ReturnType.IsGenericType)
                        service.Descriptor.ReturnType(string.Join(",",
                            methodInfo.ReturnType.GenericTypeArguments.Select(x => x.FullName)));
                    else
                        service.Descriptor.ReturnType(methodInfo.ReturnType.ToString());
                    _services.Add(service);
                }
            }

            return this;
        }

        public List<ServiceEntry> GetServiceEntry()
        {
            return _services;
        }

        private FastInvoke.FastInvokeHandler GetHandler(string key, MethodInfo method)
        {
            _handler.TryGetValue(Tuple.Create(method.DeclaringType, key), out var handler);
            if (handler == null)
            {
                handler = FastInvoke.GetMethodInvoker(method);
                _handler.GetOrAdd(Tuple.Create(method.DeclaringType, key), handler);
            }

            return handler;
        }

        private object GetInstance(string key, Type type, Payload payload)
        {
            // all service are instancePerDependency, to avoid resolve the same isntance , so we add using scop here
            using (var scope = _container.BeginLifetimeScope())
            {
                if (string.IsNullOrEmpty(key))
                    return scope.Resolve(type,
                        new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(Payload),
                            (pi, ctx) => payload
                        ));
                return scope.ResolveKeyed(key, type,
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(Payload),
                        (pi, ctx) => payload
                    ));
            }
        }
    }
}