using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Client
{
    public class ApplicationClient
    {
        public static void Run(string settingName = "JimuAppClientSettings")
        {
            new ApplicationClientBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }
        public static void RunWebHost(Action<IHostBuilder> build = null, string settingName = "JimuAppClientSettings")
        {
            var app = new ApplicationClientBuilder(new Autofac.ContainerBuilder(), settingName).Build();
            app.RunWebHost(build);
        }
    }
}
