using System;
using Autofac;
using Jimu;
using Jimu.Client;
using IServices;
using System.Threading;
using System.Diagnostics;
using Jimu.Logger;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello client!");
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1NTI1ODM4NjUsInVzZXJuYW1lIjoiYWRtaW4iLCJkZXBhcnRtZW50IjoiSVTpg6gifQ.tx4etoJenyjsujHP5QGwSlhgyl9n2ftn-UziyGIIDPo";

            var container = new ContainerBuilder();
            var host = new ApplicationClientBuilder(container)
                //.UseLog4netLogger(new JimuLog4netOptions { EnableConsoleLog = true })
                //.UsePollingAddressSelector()
                //.UseConsulForDiscovery(new Jimu.Client.Discovery.ConsulIntegration.ConsulOptions("127.0.0.1", 8500, "JimuService-"))
                //.UseHttpForTransfer()
                //.UseDotNettyForTransfer()
                .UseToken(() => token)
                //.UseServiceProxy(new Jimu.Client.Proxy.ServiceProxyOptions( new[] { "IServices" }))
                .Build();
            host.Run();

            Stopwatch watch = new Stopwatch();
            while (Console.ReadLine() != "exit")
            {
                Console.WriteLine("GetEcho with Token");
                watch.Reset();
                watch.Start();

                var proxy = host.Container.Resolve<IServiceProxy>();
                var echo = proxy.GetService<IEchoService>();
                var name = echo.GetEchoAnonymous("test");
                watch.Stop();
                Console.WriteLine($"take time {watch.ElapsedMilliseconds}," + "return:  " + name);

                Console.WriteLine("GetEchoAnonymous with no token");


                watch.Reset();
                watch.Start();

                var ret = echo.GetEchoAnonymous("test2");
                watch.Stop();
                Console.WriteLine($"take time {watch.ElapsedMilliseconds}," + "return:  " + ret);
            }

            Console.ReadKey();


        }
    }
}
