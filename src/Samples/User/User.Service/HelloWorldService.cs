using System;
using User.IService;

namespace User.Service
{
    public class HelloWorldService : IHelloWorldService
    {
        public string Get()
        {
            return "hello world!";
        }
    }
}
