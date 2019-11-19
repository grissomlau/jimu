using Jimu.Server;
using System;

namespace Auth.Server
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
