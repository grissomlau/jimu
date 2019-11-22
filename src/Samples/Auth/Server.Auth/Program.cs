using Jimu.Server;
using System;

namespace Server.Auth
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Auth Server starting ...");
            ApplicationHostServer.Instance.Run();
        }
    }
}
