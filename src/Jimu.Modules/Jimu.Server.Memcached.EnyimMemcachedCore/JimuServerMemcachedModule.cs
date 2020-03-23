using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu;
using Jimu.Module;
using Jimu.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Enyim;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using Enyim.Caching;
using Jimu.Logger;

namespace Jimu.Server.Memcached.EnyimMemcachedCore
{
    public class JimuServerMemcachedModule : ServerGeneralModuleBase
    {
        private IConfigurationSection _configurationSection;
        private MemcachedOptions _options;
        public JimuServerMemcachedModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _configurationSection = jimuAppSettings.GetSection(typeof(MemcachedOptions).Name);
            _options = jimuAppSettings.GetSection(typeof(MemcachedOptions).Name).Get<MemcachedOptions>();
        }
        public override void DoHostBuild(IHostBuilder hostBuilder)
        {
            if (_options != null && _options.Enable)
            {
                hostBuilder.ConfigureServices(services =>
                {
                    services.AddEnyimMemcached(_configurationSection);
                });
            }
            base.DoHostBuild(hostBuilder);
        }

        IContainer _jimuContainer;
        public override void DoInit(IContainer container)
        {
            _jimuContainer = container;
            base.DoInit(container);
        }

        public override void DoServiceRegister(ContainerBuilder serviceContainerBuilder)
        {
            if (_options != null && _options.Enable)
            {
                var client = _jimuContainer.Resolve<IMemcachedClient>();
                serviceContainerBuilder.Register(x => client).As<IMemcachedClient>().SingleInstance();
            }

            base.DoServiceRegister(serviceContainerBuilder);
        }

        public override void DoServiceInit(IContainer container)
        {
            var logger = container.Resolve<ILogger>();
            if (_options != null && _options.Enable)
            {
                try
                {
                    var client = container.Resolve<IMemcachedClient>();
                    client.GetValueAsync<string>("EnyimMemcached").Wait();
                    logger.Info("EnyimMemcached Register successfully.");
                }
                catch (Exception ex)
                {
                    logger.Error("Failed to UseEnyimMemcached,", ex);
                }
            }
            base.DoServiceInit(container);
        }
    }
}
