using Autofac;
using Jimu.Client;
using Jimu.Client.Proxy;
using Jimu.Server;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu
{
    public class ApplicationServerClient
    {
        public static ApplicationServerClient Instance = new ApplicationServerClient();

        Action<ApplicationServerBuilder> _serverBuilderAction;
        Action<ApplicationClientBuilder> _clientBuilderAction;

        private ApplicationServerClient()
        {
        }

        public ApplicationServerClient BuildClient(Action<ApplicationClientBuilder> action)
        {
            if (action != null)
            {
                _clientBuilderAction = action;
            }

            return this;
        }

        public ApplicationServerClient BuildServer(Action<ApplicationServerBuilder> action)
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
            ApplicationHostServer.Instance.BuildHost((hostBuilder, container) =>
            {
                thisHostBuilder = hostBuilder;
            }).BuildServer(builder =>
            {
                _serverBuilderAction?.Invoke(builder);
                IApplication hostClient = null;
                builder.AddServiceModule(x =>
                {
                    x.Register(c => hostClient.Container.Resolve<IServiceProxy>()).As<IServiceProxy>();
                }).AddRunner(x =>
                {
                    var clientBuilder = new ApplicationClientBuilder(new ContainerBuilder(), clientSettingName);
                    _clientBuilderAction?.Invoke(clientBuilder);
                    hostClient = clientBuilder.Build();
                    hostClient.RunInServer(thisHostBuilder);
                });
            }).Run(serverSettingName);
        }
    }
}
