using Autofac;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Server
{
    public class ApplicationServer
    {
        public static void Run(string settingName = "JimuAppServerSettings")
        {
            new ApplicationServerBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }

        public static void RunGeneralHost(Action<IHostBuilder, IContainer> build = null, string settingName = "JimuAppServerSettings")
        {
            var app = new ApplicationServerBuilder(new Autofac.ContainerBuilder(), settingName).Build();
            app.RunGeneralHost(build);
        }
    }
}
