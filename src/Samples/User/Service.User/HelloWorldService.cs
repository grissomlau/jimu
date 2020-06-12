using IService.User;
using IService.User.dto;
using System.Collections.Generic;
using System.Threading;

namespace Service.User
{
    public class HelloWorldService : IHelloWorldService
    {
        public void FastTask()
        {

        }

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

        public void LongTask(int count)
        {
            if (count % 2 == 0)
                Thread.Sleep(10000);
        }
    }
}
