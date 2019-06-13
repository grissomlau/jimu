using IServices;
using Jimu;
using Jimu.Client;
using System;

namespace TestService
{
    [JimuServiceRoute("api/{Service}")]
    public class HelloService : IJimuService
    {
        readonly IServiceProxy _serviceProxy;
        public HelloService(IServiceProxy proxy)
        {
            _serviceProxy = proxy;
        }

        [JimuService()]
        public string GetEcho()
        {
            var echoService = _serviceProxy.GetService<IEchoService>();
            var ret = echoService.GetEchoAnonymous("echo from hello service!");
            return "hello return: " + ret;
        }
    }
}
