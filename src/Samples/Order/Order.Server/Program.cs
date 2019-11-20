using Autofac;
using Jimu;
using Jimu.Client;
using Jimu.Client.Proxy;
using Jimu.Server;
using Microsoft.Extensions.Hosting;
using System;

namespace Order.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Order Server start...");
            ApplicationServerClient.Instance.Run();

        }
    }
}
