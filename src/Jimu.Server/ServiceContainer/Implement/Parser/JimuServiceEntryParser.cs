using Autofac;
using Autofac.Core;
using Jimu.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Jimu.Server.ServiceContainer.Implement.Parser
{
    class JimuServiceEntryParser
    {
        private readonly IContainer _container;
        public JimuServiceEntryParser(IContainer container)
        {
            _container = container;
        }

        public JimuServiceEntry Parse(MethodInfo methodInfo, JimuServiceDesc desc)
        {
            var fastInvoker = FastInvoke.GetMethodInvoker(methodInfo);

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
                        var parameter = JimuHelper.ConvertType(value, paraType);
                        parameters.Add(parameter);
                    }

                    var result = fastInvoker(instance, parameters.ToArray());
                    return Task.FromResult(result);
                }
            };
            return service;
        }

        private object GetInstance(string key, Type type, JimuPayload payload)
        {
            // all service are instancePerDependency, to avoid resolve the same isntance , so we use scop here
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
