using Autofac;
using Jimu.Client.ApiGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu.Client
{
    public class ApplicationClient
    {
        public static ApplicationClient Instance = new ApplicationClient();
        private ApplicationClient()
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
        Action<ApplicationClientBuilder> _clientBuilderAction;
        private ApplicationWebClient()
        {
        }

        public ApplicationWebClient BuildClient(Action<ApplicationClientBuilder> action)
        {
            if (action != null)
            {
                _clientBuilderAction = action;
            }

            return this;
        }
        public ApplicationWebClient BuildWebHost(Action<IHostBuilder> action)
        {
            _hostBuilderAction = action;
            return this;
        }
        public void Run(Action<IServiceCollection> configureServicesAction = null, Action<WebHostBuilderContext, IApplicationBuilder> configureAction = null, string settingName = "JimuAppClientSettings")
        {
            var hostBuilder = Host.CreateDefaultBuilder();
            _hostBuilderAction?.Invoke(hostBuilder);

            var clientBuilder = new ApplicationClientBuilder(new ContainerBuilder(), settingName);
            _clientBuilderAction?.Invoke(clientBuilder);

            hostBuilder.UseWebHostJimu(settingName, configureServicesAction, configureAction, clientBuilder.Build());
            hostBuilder.Build().Run();
        }
    }
}
