using Autofac;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Jimu.Client
{
    public static class ApplicationExtension
    {

        public static void RunInServer(this IApplication app, IHostBuilder hostBuilder = null)
        {
            if (hostBuilder != null)
            {
                var type = typeof(ClientGeneralModuleBase);
                var hostModules = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                    .Select(x => Activator.CreateInstance(x, app.JimuAppSettings) as ClientGeneralModuleBase)
                    .OrderBy(x => x.Priority);
                foreach (var module in hostModules)
                {
                    module.DoHostBuild(hostBuilder, app.Container);
                }
            }
            app.Run();
        }

    }
}
