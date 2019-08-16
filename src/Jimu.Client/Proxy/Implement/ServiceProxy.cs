using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jimu.Client
{
    public class ServiceProxy : IServiceProxy
    {
        private readonly IRemoteServiceCaller _remoteServiceCaller;
        private readonly IList<Type> _serviceProxyTypes;

        public ServiceProxy(IServiceProxyGenerator generator, IRemoteServiceCaller remoteServiceCaller)
        {
            _serviceProxyTypes = generator.GetGeneratedServiceProxyTypes().ToList();
            _remoteServiceCaller = remoteServiceCaller;
        }

        public T GetService<T>(JimuPayload payload = null) where T : class
        {
            //var instanceType = typeof(T);
            var proxyType = _serviceProxyTypes.Single(typeof(T).GetTypeInfo().IsAssignableFrom);
            var instance = proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[]
            {
                _remoteServiceCaller,
                payload
            });
            return instance as T;
        }
    }
}