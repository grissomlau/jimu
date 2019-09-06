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

        [JimuService(AllowAnonymous = true)]
        public string Hi2()
        {
            return "hi";
        }
        [JimuService(RoutePath = "/api/hello/hi2?name={name}", AllowAnonymous = true)]
        public string Hi2(string name)
        {
            return $"hi, {name}";
        }
        [JimuService(RestPath = "/echo/{name}", AllowAnonymous = true)]
        public string Echo(string name, string from)
        {
            return name + from;
        }
        [JimuService(RestPath = "/{id}/{name}", AllowAnonymous = true)]
        public string Get(int id, string name)
        {
            return "get: " + id + ", name: " + name;
        }

        [JimuService(RestPath = "/users/{uid}/friends/{fid}", AllowAnonymous = true)]
        public string Get(int uid, int fid)
        {
            return $"get2 uid: {uid}, fid: {fid}";
        }

        [JimuService(HttpMethod = "POST", AllowAnonymous = true)]
        public string Post(string id, User user)
        {
            return $"post uname: {user.Name5}, usernam2: {user.Name}";
        }
        [JimuService(RestPath = "/{id}", HttpMethod = "PUT", AllowAnonymous = true)]
        public string Put(int id, User user)
        {
            return $"put uid: {id}, user: {user.Name}";
        }

        [JimuService(RestPath = "/{id}", HttpMethod = "DELETE", AllowAnonymous = true)]
        public string Delete(int id)
        {
            return $"delete uid: {id}";
        }
        [JimuService()]
        public string GetEcho()
        {
            JimuPayload payload = new JimuPayload
            {
                Items = new Dictionary<string, object>()
            };
            payload.Items["userid"] = "fuck";
            var echoService = _serviceProxy.GetService<IEchoService>(payload);
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
        [JimuService(AllowAnonymous = true)]
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
