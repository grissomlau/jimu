using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;

namespace Jimu.Server
{
    public class ServiceEntryContainer : IServiceEntryContainer
    {
        private readonly IContainer _container;
        private readonly IServiceIdGenerator _serviceIdGenerate;
        private readonly ITypeConvertProvider _typeConvertProvider;
        private readonly ConcurrentDictionary<Tuple<Type, string>, FastInvoke.FastInvokeHandler> _handler;
        private readonly List<JimuServiceEntry> _services;

        public ServiceEntryContainer(IContainer container, IServiceIdGenerator serviceIdGenerate,
                ITypeConvertProvider typeConvertProvider)
        //public ServiceEntryContainer()
        {
            _serviceIdGenerate = serviceIdGenerate;
            _container = container;
            _typeConvertProvider = typeConvertProvider;
            _services = new List<JimuServiceEntry>();
            _handler = new ConcurrentDictionary<Tuple<Type, string>, FastInvoke.FastInvokeHandler>();
            new ConcurrentDictionary<Tuple<Type, string>, object>();
        }


        public IServiceEntryContainer AddServices(Type[] types)
        {
            //var serviceTypes = types.Where(x =>
            //{
            //    var typeinfo = x.GetTypeInfo();
            //    return typeinfo.IsInterface && typeinfo.GetCustomAttribute<JimuServiceRouteAttribute>() != null;
            //}).Distinct();

            var serviceTypes = types
                .Where(x => x.GetMethods().Any(y => y.GetCustomAttribute<JimuServiceAttribute>() != null)).Distinct();

            foreach (var type in serviceTypes)
            {
                var routeTemplate = type.GetCustomAttribute<JimuServiceRouteAttribute>();
                foreach (var methodInfo in type.GetTypeInfo().GetMethods().Where(x => x.GetCustomAttributes<JimuServiceDescAttribute>().Any() ))
                {

                    JimuServiceDesc desc = new JimuServiceDesc();
                    var descriptorAttributes = methodInfo.GetCustomAttributes<JimuServiceDescAttribute>();
                    foreach (var attr in descriptorAttributes) attr.Apply(desc);
                    if (methodInfo.ReturnType.ToString().IndexOf("System.Threading.Tasks.Task", StringComparison.Ordinal) == 0 &&
                        methodInfo.ReturnType.IsGenericType)
                        desc.ReturnType = string.Join(",", methodInfo.ReturnType.GenericTypeArguments.Select(x => x.FullName));
                    else
                        desc.ReturnType = methodInfo.ReturnType.ToString();

                    if (string.IsNullOrEmpty(desc.Id))
                    {
                        desc.Id = _serviceIdGenerate.GenerateServiceId(methodInfo);
                    }

                    var fastInvoker = GetHandler(desc.Id, methodInfo);
                    if (routeTemplate != null)
                        desc.RoutePath = JimuServiceRoute.ParseRoutePath(routeTemplate.RouteTemplate, type.Name,
                            methodInfo.Name, methodInfo.GetParameters());

                    var service = new JimuServiceEntry
                    {
                        Descriptor = desc,
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

                    _services.Add(service);
                }
            }

            return this;
        }

        public List<JimuServiceEntry> GetServiceEntry()
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

        private object GetInstance(string key, Type type, JimuPayload payload)
        {
            // all service are instancePerDependency, to avoid resolve the same isntance , so we add using scop here
            using (var scope = _container.BeginLifetimeScope())
            {
                if (string.IsNullOrEmpty(key))
                    return scope.Resolve(type,
                        new ResolvedParameter(
                            (pi, ctx) => pi.ParameterType == typeof(JimuPayload),
                            (pi, ctx) => payload
                        ));
                return scope.ResolveKeyed(key, type,
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(JimuPayload),
                        (pi, ctx) => payload
                    ));
            }
        }
    }
}