using System;
using Autofac;
using Jimu;
using Jimu.Common.Logger.Log4netIntegration;
using Jimu.Common.Transport.DotNettyIntegration;
using Jimu.Core.Server;
using Jimu.Core.Server.ServiceContainer;
using Jimu.Server.OAuth.JwtIntegration.Middlewares;
using DbWorker.IUnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace DDD.Simple.Server
{
    //    public class BloggingContext : DbContext
    //    {
    //        public BloggingContext(DbContextOptions<BloggingContext> options) : base(options)
    //{

    //        }
    //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        {
    //            optionsBuilder.UseMySql("Data Source=blog.db");
    //        }
    //    }

    class Program
    {

        static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            //var op = new DbContextOptions<BloggingContext>();

            //using (var context = new BloggingContext(op))
            //{
            //    var name = context.Database.ProviderName;
            //    // do stuff
            //}
            Console.WriteLine("Hello World!");
            Configuration = new ConfigurationBuilder()
                 .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                 .Build();
            Log4netOptions log4NetOptions = Configuration.GetSection("Log4netOptions").Get<Log4netOptions>();
            //DbOptions dbOptions = Configuration.GetSection("DbOptions").Get<DbOptions>();

            DbContextOption dbContextOption = Configuration.GetSection("DbContextOption").Get<DbContextOption>();


            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .UseLog4netLogger(log4NetOptions)
                .RegisterService(container =>
                {
                    // inject dboptions
                    //container.Register(x => dbOptions);
                    container.Register(x => dbContextOption);
                    container.Register(x => dbContextOption).SingleInstance();
                })
                //.LoadServices(new string[] { "DDD.Simple.IServices", "DDD.Simple.Services" }, "DDD.Simple.IServices.*.*Service|DDD.Simple.Services.*.*Service")
                .LoadServices(new[] { "DDD.Simple.IServices", "DDD.Simple.Services", "DDD.Simple.Repository.SqlSugar", "DbWorker.UnitOfWork.SqlSugar" })
                 //.LoadServices(new string[] { "DDD.Simple.IServices", "DDD.Simple.Services", "DDD.Simple.Repository.EF", "DbWorker.UnitOfWork.EF" })
                 //.UseDotNettyServer("127.0.0.1", 8009, server => { })
                 .UseNetCoreHttpServer("127.0.0.1", 8009, server => { })
                 .UseJwtAuthorization<DotNettyAddress>(new JwtAuthorizationOptions
                 {
                     CheckCredential = context =>
                      {
                          if (context.UserName == "admin" && context.Password == "admin")
                          {
                              context.AddClaim("username", "DbWorker");
                              context.AddClaim("dep", "IT");
                              context.AddClaim("role", "admin");
                          }
                          else
                          {
                              context.Rejected("username or password are incorrect.", "用户名或密码错误");
                          }
                      },
                     ValidateLifetime = true,
                     ExpireTimeSpan = TimeSpan.FromDays(30),
                     SecretKey = "12345678901234567890123456789012",
                     ServerIp = "127.0.0.1",
                     ServerPort = 8009,
                     TokenEndpointPath = "oauth/token?username=&password="
                 })
                .UseConsul("127.0.0.1", 8500, "MService-", "127.0.0.1:8009")
                ;
            using (var host = builder.Build())
            {
                // 要使用 migration 必须要调用对象
                // EF 专享
                //var context = host.Container.Resolve<DefaultDbContext>();
                //var name = context.Database.ProviderName;
                Console.WriteLine("Server start successful.");
                host.Run();
                Console.ReadLine();
            }
        }
    }
}
