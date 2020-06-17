using IService.Auth;
using Jimu;
using Jimu.Server.Auth.Middlewares;
using Jimu.UnitOfWork;
using System;
using System.Data.Common;

namespace Service.Auth
{
    public class AuthService : IAuthService
    {
        readonly IDbFactory<DbConnection> _dbFactory;
        readonly JimuPayload _payload;
        public AuthService(IDbFactory<DbConnection> dbFactory, JimuPayload payload)
        {
            _dbFactory = dbFactory;
            _payload = payload;
        }
        public void Check(JwtAuthorizationContext context)
        {
            Console.WriteLine("open database");

            //// 1.using database
            //using (var cnn = _dbFactory.Create())
            //{
            //    var auth = cnn.QuerySingleOrDefault<User>(@"Select id, name, email From users Where email = @email and pwd = @pwd", new { email = context.UserName, pwd = context.Password });
            //    if (auth != null)
            //    {
            //        context.AddClaim("email", auth.Email);
            //    }
            //}


            // 2.hardcode
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
