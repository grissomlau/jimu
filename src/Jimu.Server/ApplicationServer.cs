using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Jimu.Server
{
    public class ApplicationServer
    {
        public static ApplicationServer Instance = new ApplicationServer();
        private ApplicationServer()
        {
        }
        public void Run(string settingName = "JimuAppServerSettings")
        {
            new ApplicationServerBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }
    }

    public class ApplicationHostServer
    {
        public static ApplicationHostServer Instance = new ApplicationHostServer();
        private Action<IHostBuilder, ContainerBuilder> _hostBuilderAction = null;
        private Action<ApplicationServerBuilder> _serverBuilderAction = null;
        private ApplicationHostServer()
        {

        }

        public ApplicationHostServer BuildServer(Action<ApplicationServerBuilder> action)
        {
            if (action != null)
            {
                _serverBuilderAction = action;
            }

            return this;
        }

        public ApplicationHostServer BuildHost(Action<IHostBuilder, ContainerBuilder> action)
        {
            _hostBuilderAction = action;
            return this;
        }


        public void Run(string settingName = "JimuAppServerSettings")
        {
            var containerBuilder = new Autofac.ContainerBuilder();
            var serverBuilder = new ApplicationServerBuilder(containerBuilder, settingName);
            var hostBuilder = new HostBuilder();
            IHost host = null;

            serverBuilder.AddRegister(cb =>
            {
                var type = typeof(ServerGeneralModuleBase);
                var hostModules = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                    .Select(x => Activator.CreateInstance(x, serverBuilder.JimuAppSettings) as ServerGeneralModuleBase)
                    .OrderBy(x => x.Priority);
                foreach (var module in hostModules)
                {
                    module.DoHostBuild(hostBuilder);
                }

                _hostBuilderAction?.Invoke(hostBuilder, containerBuilder);
                _serverBuilderAction?.Invoke(serverBuilder);

                hostBuilder.ConfigureServices(sc =>
                {
                    serverBuilder.AddBeforeBuilder(cb =>
                    {
                        cb.Populate(sc);
                    });
                });

                host = hostBuilder.Build();
            });



            var app = serverBuilder.Build();

            app.Run();
            host?.Run();
        }
    }
}
