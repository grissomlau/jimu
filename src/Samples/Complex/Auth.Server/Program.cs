using System;
using Autofac;
using Jimu;
using Jimu.Server;

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

            ApplicationServer.Run();
            Console.ReadLine();
        }
    }
}
