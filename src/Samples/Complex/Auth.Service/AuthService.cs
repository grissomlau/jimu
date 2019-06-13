using Auth.IService;
using Auth.IService.DTO;
using System;
using System.Data.Common;
using Dapper;
using Jimu.Server.Auth;
using Jimu;

namespace Auth.Service
{
    public class AuthService : IAuthService
    {
        readonly DbConnection _cnn;
        readonly JimuPayload _payload;
        public AuthService(DbConnection cnn, JimuPayload payload)
        {
            _cnn = cnn;
            _payload = payload;
        }
        public void Check(JwtAuthorizationContext context)
        {
            Console.WriteLine("open database");
            /*       
             *       // using database
             *       _cnn.Open();
                       var auth = _cnn.QuerySingleOrDefault<User>(@"Select id, name, email From users Where email = @email and pwd = @pwd", new { email = context.UserName, pwd = context.Password });
                       if (auth != null)
                       {
                           context.AddClaim("email", auth.Email);
                       }
                       */

            // hardcode
            if (context.UserName == "admin" && context.Password == "admin")
            {
                context.AddClaim("username", "admin");
                //context.AddClaim("userid", "");
            }
            else
            {
                context.Rejected("username or password error", "please check your username and password!");
            }

        }

        public string GetCurrentUserName()
        {
            return _payload.Items["username"] + "";
        }
    }
}
