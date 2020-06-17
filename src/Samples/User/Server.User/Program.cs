using Jimu.Server;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;

namespace User.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("User Server starting ...");
            //register db provider
            DbProviderFactories.RegisterFactory("mysql", MySqlClientFactory.Instance);
            ApplicationHostServer.Instance.Run();
        }
    }
}
