using Autofac;
using Jimu.Client;
using Jimu.Client.Proxy;
using Jimu.Server;
using Microsoft.Extensions.Hosting;
using System;

namespace Jimu
{
    public class ApplicationMix
    {
        public static ApplicationMix Instance = new ApplicationMix();

        Action<ApplicationServerBuilder> _serverBuilderAction;
        Action<ApplicationClientBuilder> _clientBuilderAction;

        private ApplicationMix()
        {
        }

        public ApplicationMix BuildClient(Action<ApplicationClientBuilder> action)
        {
            if (action != null)
            {
                _clientBuilderAction = action;
            }

            return this;
        }

        public ApplicationMix BuildServer(Action<ApplicationServerBuilder> action)
        {
            if (action != null)
            {
                _serverBuilderAction = action;
            }

            return this;
        }
        public void Run(string clientSettingName = "JimuAppClientSettings", string serverSettingName = "JimuAppServerSettings")
        {
            IHostBuilder thisHostBuilder = null;
            ApplicationHostServer.Instance.BuildHost((hostBuilder, containerBuilder) =>
            {
                thisHostBuilder = hostBuilder;
            }).BuildServer(builder =>
            {
                _serverBuilderAction?.Invoke(builder);
                IApplication hostClient = null;
                builder.AddServiceRegister(x =>
                {
                    x.Register(c => hostClient.Container.Resolve<IServiceProxy>()).As<IServiceProxy>();
                }).AddRunner(x =>
                {
                    var clientBuilder = new ApplicationClientBuilder(new ContainerBuilder(), clientSettingName);
                    _clientBuilderAction?.Invoke(clientBuilder);
                    clientBuilder.BuildHostModule(thisHostBuilder);
                    hostClient = clientBuilder.Build();
                    hostClient.Run();
                });
            }).Run(serverSettingName);
        }
    }
}
