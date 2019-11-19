using Jimu;
using System;
using System.Collections.Generic;
using System.Text;
using User.IService;

namespace User.Service
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
