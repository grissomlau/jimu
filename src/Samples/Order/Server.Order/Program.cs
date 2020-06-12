using Jimu;
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
