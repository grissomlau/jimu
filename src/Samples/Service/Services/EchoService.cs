using System.Threading.Tasks;
using Jimu;
using IServices;
using System.Collections.Generic;

namespace Services
{
    public class EchoService : IEchoService
    {

        private readonly ILogger _logger;
        private readonly JimuPayload _payload;
        public EchoService(ILogger logger, JimuPayload payload)
        {
            _logger = logger;
            _payload = payload;
        }

        public string CreateUser(UserDTO user)
        {
            return "ok";
        }

        public string CreateUserFriend(UserDTO user, UserFriendDTO friend, string comment)
        {
            return "ok";
        }

        public string GetEcho(string anything)
        {
            return $"the echo is  {anything}";
        }

        public Task<string> SetEcho(string anything)
        {
            //return Task.FromResult(new UserDTO());
            return Task.FromResult($"the echo is {anything} and the current user  is {_payload.Items["username"]}");
        }

        //public Task<UserDTO> SetEcho(string anything, List<string> anything2)
        public List<UserDTO> SetEcho(string anything, string anything2)
        {
            throw new System.NotImplementedException();
        }
    }
}
