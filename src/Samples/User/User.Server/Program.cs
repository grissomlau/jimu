using Jimu.Server;
using System;

namespace User.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("User Server starting ...");
            ApplicationServer.Run();
            Console.ReadLine();

            // if run in docker, uncomment bellow code 
            //var host = new HostBuilder();
            //host.RunConsoleAsync().GetAwaiter().GetResult();
        }
    }
}
