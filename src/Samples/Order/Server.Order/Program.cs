using Jimu;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace Server.Order
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Order Server start...");
            //register db provider
            DbProviderFactories.RegisterFactory("mysql", MySqlClientFactory.Instance);
            ApplicationMix.Instance.Run();

        }
    }
}
