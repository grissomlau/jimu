using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using IServices;
using Jimu;
using Jimu.Client;
using Jimu.Server;

namespace Server1
{
    class Program
    {
        static IEchoService _echoService;
        static void Main(string[] args)
        {
            var containerBuilder = new ContainerBuilder();
            var hostBuilder = new ApplicationServerBuilder(containerBuilder)
                    .LoadServices(new[] { "IServices", "Services" })
                    .UseLog4netLogger(new LogOptions { EnableConsoleLog = true })
                    //.UseHttpForTransfer("127.0.0.1", 8007)// http server ip and port,becareful the firewall blocker
                    .UseConsulForDiscovery(new Jimu.Server.Discovery.ConsulIntegration.ConsulOptions("127.0.0.1", 8500, "JimuService-", "127.0.0.1:8009"))
                    .UseDotNettyForTransfer(new Jimu.Server.Transport.DotNetty.DotNettyOptions("127.0.0.1", 8009))
                                                 .UseJoseJwtForOAuth<DotNettyAddress>(new Jimu.Server.Auth.JwtAuthorizationOptions
                                                 {
                                                     ServerIp = "127.0.0.1",
                                                     ServerPort = 8009,
                                                     SecretKey = "test",
                                                     ExpireTimeSpan = new TimeSpan(1, 0, 0),
                                                     TokenEndpointPath = "token",
                                                     ValidateLifetime = true,
                                                     CheckCredential = o =>
                                                     {
                                                         if (o.UserName == "admin" && o.Password == "admin")
                                                         {
                                                             o.AddClaim("department", "IT部");
                                                         }
                                                         else
                                                         {
                                                             o.Rejected("401", "acount or password incorrect");
                                                         }
                                                     }
                                                 })
                ;
            using (var host = hostBuilder.Build())
            {
                //InitProxyService();
                host.Run();
                Console.ReadLine();
            }
        }

        static void InitProxyService()
        {
            var containerBuilder = new ContainerBuilder();
            var host = new Jimu.Client.ApplicationClientBuilder(containerBuilder)
                //.UseLog4netLogger(new LogOptions { EnableConsoleLog = true })
                .UsePollingAddressSelector()
                .UseConsulForDiscovery(new Jimu.Client.Discovery.ConsulIntegration.ConsulOptions("127.0.0.1", 8500, "JimuService-"))
                .UseDotNettyForTransfer()
                .UseHttpForTransfer()
                .UseServiceProxy(new Jimu.Client.Proxy.ServiceProxyOptions(new[] { "IServices" }))
                .Build()
                ;
            host.Run();
            var proxy = host.Container.Resolve<IServiceProxy>();
            _echoService = proxy.GetService<IEchoService>();
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                var ret = _echoService.GetEcho("哈哈");
                Console.WriteLine("==== echo " + ret);
            });
        }
    }
}
