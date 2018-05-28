using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jimu.Core.Client.RemoteInvoker;

namespace Jimu.Core.Client.Proxy
{
    public class ServiceProxy : IServiceProxy
    {
        private static ServiceProxy _serviceProxy;
        private readonly IRemoteServiceInvoker _remoteServiceInvoker;
        private readonly IList<Type> _serviceProxyTypes;

        public ServiceProxy(IServiceProxyGenerator generator, IRemoteServiceInvoker remoteServiceInvoker)
        {
            _serviceProxyTypes = generator.GetGeneratedServiceProxyTypes().ToList();
            _remoteServiceInvoker = remoteServiceInvoker;
            _serviceProxy = this;
        }

        public T GetServiceByType<T>() where T : class
        {
            //var instanceType = typeof(T);
            var proxyType = _serviceProxyTypes.Single(typeof(T).GetTypeInfo().IsAssignableFrom);
            var instance = proxyType.GetTypeInfo().GetConstructors().First().Invoke(new object[]
            {
                _remoteServiceInvoker
            });
            return instance as T;
        }

        public static T GetService<T>() where T : class
        {
            return _serviceProxy.GetServiceByType<T>();
        }
    }
}