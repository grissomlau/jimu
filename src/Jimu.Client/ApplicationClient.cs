using Autofac;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Client
{
    public class ApplicationGeneralClient
    {
        public static ApplicationGeneralClient Instance = new ApplicationGeneralClient();
        private ApplicationGeneralClient()
        {
        }
        public void Run(string settingName = "JimuAppClientSettings")
        {
            new ApplicationClientBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }


    }
    public class ApplicationWebClient
    {
        public static ApplicationWebClient Instance = new ApplicationWebClient();
        private Action<IHostBuilder> _hostBuilderAction = null;
        private ApplicationWebClient()
        {
        }
        public void Run(string settingName = "JimuAppClientSettings")
        {
            new ApplicationClientBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }

        public ApplicationWebClient BuildWebHost(Action<IHostBuilder> action)
        {
            _hostBuilderAction = action;
            return this;
        }
        public void RunWebHost(Action<IServiceCollection> configureServicesAction = null, Action<WebHostBuilderContext, IApplicationBuilder> configureAction = null, string settingName = "JimuAppClientSettings")
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            _hostBuilderAction?.Invoke(hostBuilder);

            hostBuilder.UseWebHostJimu(settingName, configureServicesAction, configureAction);
            hostBuilder.Build().Run();
        }
    }
}
