using IServices;
using Jimu;
using Jimu.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        //[JimuService()]
        public string GetEcho()
        {
            var echoService = _serviceProxy.GetService<IEchoService>();
            var ret = echoService.GetEchoAnonymous2("echo from hello service!");
            return "hello return: " + ret.Name;
        }

        //[JimuService(EnableAuthorization = false)]
        public List<User> GetComplexReturn1()
        {
            return new List<User>();
            //return MsgModel<User>.Success(new User());
        }


        /// <summary>
        /// hehe2
        /// </summary>
        /// <param name="user2">用户</param>
        /// <param name="foo">foof</param>
        [JimuService(EnableAuthorization = false)]
        public Task<MsgModel<User>> GetComplexReturn(User user2, string foo)
        {
            return Task.FromResult(MsgModel<User>.Success(new User()));
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
      /// <summary>
        /// 很好
        /// </summary>
        public List<string> Name5 { get; set; }
         /// <summary>
        /// name7
        /// </summary>
        public DateTime Name7 { get; set; }

        /// <summary>
        /// 孩子
        /// </summary>
        //public User Child { get; set; }
    }
}
