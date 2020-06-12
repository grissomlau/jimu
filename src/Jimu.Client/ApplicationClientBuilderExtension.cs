using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace Jimu.Client
{
    public static class ApplicationClientBuilderExtension
    {

        public static void BuildHostModule(this ApplicationClientBuilder appBuilder, IHostBuilder hostBuilder = null)
        {
            if (hostBuilder != null)
            {
                var type = typeof(ClientGeneralModuleBase);
                var hostModules = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && type.IsAssignableFrom(x) && !x.IsAbstract)
                    .Select(x => Activator.CreateInstance(x, appBuilder.JimuAppSettings) as ClientGeneralModuleBase)
                    .OrderBy(x => x.Priority);
                foreach (var module in hostModules)
                {
                    module.DoHostBuild(hostBuilder);
                }
                hostBuilder.ConfigureServices(sc => appBuilder.AddBeforeBuilder((cb) => cb.Populate(sc)));
            }
        }

    }
}
