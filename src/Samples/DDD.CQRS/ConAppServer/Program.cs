using System;
using Autofac;
using Jimu.Server;

namespace DDD.CQRS.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .LoadServices(new[] { "DDD.CQRS.IServices", "DDD.CQRS.Services" })
                .UseMasstransit(new MassTransitOptions
                {
                    HostAddress = new Uri("rabbitmq://localhost/"),
                    Username = "guest",
                    Password = "guest",
                    QueueName = "Jimu_test",
                    SendEndPointUri = new Uri("rabbitmq://localhost/Jimu_test")
                })
                .UseDotNettyServer("127.0.0.1", 8009, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "MService-", "127.0.0.1:8009")
                ;
            using (var host = builder.Build())
            {
                host.Run();
                Console.WriteLine("Server start successful.");
                Console.ReadLine();
            }
        }
    }
}
