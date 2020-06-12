using Jimu.Server;
using System;

namespace User.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("User Server starting ...");
            ApplicationHostServer.Instance.Run();
        }
    }
}
