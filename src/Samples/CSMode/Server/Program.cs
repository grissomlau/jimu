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
            var hostBuilder = new ServiceHostServerBuilder(containerBuilder)
                    .LoadServices(new[] { "IServices", "Services" })
                    .UseLog4netLogger(new LogOptions { EnableConsoleLog = true })
                    .UseHttpForTransfer("127.0.0.1", 8007)// http server ip and port,becareful the firewall blocker
                    .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-", "127.0.0.1:8007")
                ;
            using (var host = hostBuilder.Build())
            {
                InitProxyService();
                host.Run();
            }
        }

        static void InitProxyService()
        {
            var containerBuilder = new ContainerBuilder();
            var host = new Jimu.Client.ServiceHostClientBuilder(containerBuilder)
                //.UseLog4netLogger(new LogOptions { EnableConsoleLog = true })
                .UsePollingAddressSelector()
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService-")
                .UseDotNettyForTransfer()
                .UseHttpForTransfer()
                .UseServiceProxy(new[] { "IServices" })
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
