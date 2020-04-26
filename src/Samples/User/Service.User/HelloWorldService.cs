using IService.User;
using IService.User.dto;
using System;
using System.Collections.Generic;

namespace Service.User
{
    public class HelloWorldService : IHelloWorldService
    {
        public string Get()
        {
            return "hello world!";
        }

        public List<string> GetUserByArray(List<string> req)
        {
            return req;
        }

        public UserModel GetUserByObj(UserReq req)
        {
            return new UserModel { Id = req.Id, Name = req.Name };
        }
    }
}
