using IService.User;
using Jimu;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.User
{
    public class JwtService : IJwtService
    {
        readonly JimuPayload _payload;
        public JwtService(JimuPayload payload)
        {
            _payload = payload;
        }
        public string Get()
        {
            return $"username {_payload?.Items?["username"]},success!";
        }
    }
}
