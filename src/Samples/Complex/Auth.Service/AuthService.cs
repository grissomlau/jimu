using Auth.IService;
using Auth.IService.DTO;
using System;
using System.Data.Common;
using Dapper;
using Jimu.Server.Auth;

namespace Auth.Service
{
    public class AuthService : IAuthService
    {
        readonly DbConnection _cnn;
        public AuthService(DbConnection cnn)
        {
            _cnn = cnn;
        }
        public void Check(JwtAuthorizationContext context)
        {
            Console.WriteLine("open database");
            _cnn.Open();
            var auth = _cnn.QuerySingleOrDefault<User>(@"Select id, name, email From users Where email = @email and pwd = @pwd", new { email = context.UserName, pwd = context.Password });
            if (auth != null)
            {
                context.AddClaim("email", auth.Email);
            }
        }
    }
}
