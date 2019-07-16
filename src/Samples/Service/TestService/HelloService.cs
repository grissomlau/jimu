using IServices;
using Jimu;
using Jimu.Client;
using System;
using System.Collections.Generic;

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

        [JimuService(EnableAuthorization = false)]
        public List<User> GetComplexReturn1()
        {
            return new List<User>();
            //return MsgModel<User>.Success(new User());
        }


        [JimuService(EnableAuthorization = false)]
        public MsgModel<User> GetComplexReturn()
        {
            return MsgModel<User>.Success(new User());
        }

    }

    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }
    }
}
