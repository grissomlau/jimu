using Autofac;
using Jimu;
using Jimu.Client;
using Jimu.Client.Proxy;
using Jimu.Server;
using Microsoft.Extensions.Hosting;
using System;

namespace Server.Order
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Order Server start...");
            ApplicationMix.Instance.Run();

        }
    }
}
