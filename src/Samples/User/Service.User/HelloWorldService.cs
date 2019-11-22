using IService.User;
using System;

namespace Service.User
{
    public class HelloWorldService : IHelloWorldService
    {
        public string Get()
        {
            return "hello world!";
        }
    }
}
