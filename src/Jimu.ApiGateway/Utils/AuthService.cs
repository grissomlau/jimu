using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Jose;

namespace Jimu.ApiGateway
{
    public class AuthService
    {
        readonly byte[] _key;
        public AuthService(byte[] key)
        {
            _key = key;
        }
        public Task<Dictionary<string, object>> TryLogin(string username, string pwd)
        {
            var claims = new Dictionary<string, object>
            {
                { "exp", DateTime.Now.ToInt() },
                { "username", "admin" }
            };
            return Task.FromResult(claims);
        }
        public string CreateToken(Dictionary<string, object> claim)
        {
            var token = JWT.Encode(claim, _key, JwsAlgorithm.HS256);
            return token;
        }

    }
}
