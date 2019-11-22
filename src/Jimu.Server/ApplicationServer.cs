using Autofac;
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
        private Action<IHostBuilder, IContainer> _hostBuilderAction = null;
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

        public ApplicationHostServer BuildHost(Action<IHostBuilder, IContainer> action)
        {
            _hostBuilderAction = action;
            return this;
        }


        public void Run(string settingName = "JimuAppServerSettings")
        {
            var serverBuilder = new ApplicationServerBuilder(new Autofac.ContainerBuilder(), settingName);

            _serverBuilderAction?.Invoke(serverBuilder);

            var app = serverBuilder.Build();

            var hostBuilder = new HostBuilder();

            var type = typeof(ServerGeneralModuleBase);
            var hostModules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                .Select(x => Activator.CreateInstance(x, app.JimuAppSettings) as ServerGeneralModuleBase)
                .OrderBy(x => x.Priority);
            foreach (var module in hostModules)
            {
                module.DoHostBuild(hostBuilder, app.Container);
            }

            _hostBuilderAction?.Invoke(hostBuilder, app.Container);
            app.Run();
            hostBuilder.Build().Run();
        }
    }
}
