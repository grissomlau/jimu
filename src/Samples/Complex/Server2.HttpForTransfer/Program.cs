using Jimu.Server;
using System;

namespace Server2.HttpForTransfer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //var containerBuilder = new ContainerBuilder();
            //var builder = new ApplicationServerBuilder(containerBuilder)
            //    //.UseLog4netLogger(new JimuLog4netOptions
            //    //{
            //    //    EnableConsoleLog = true
            //    //})
            //    //.LoadServices(new[] { "IServices", "Services" })
            //    //.UseHttpForTransfer(new Jimu.Server.Transport.Http.HttpOptions("127.0.0.1", 8006))
            //    ;
            //using (var hostJimu = builder.Build())
            //{
            //    hostJimu.Run();
            //}

            ApplicationServer.Run();

        }
    }
}
