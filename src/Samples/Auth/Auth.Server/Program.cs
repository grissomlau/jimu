using Jimu.Server;
using System;

namespace Auth.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Auth Server starting ...");
            //var containerBuilder = new ContainerBuilder();
            //var builder = new ApplicationServerBuilder(containerBuilder);
            //using (var hostJimu = builder.Build())
            //{
            //    hostJimu.Run();
            //}

            ApplicationServer.Instance.Run();
            Console.ReadLine();
        }
    }
}
