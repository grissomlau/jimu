using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm;
using SkyApm.Agent.GeneralHost;

namespace Jimu.Client.Diagnostic.Skywalking
{
    public class SkywalkingClientModule : ClientWebModuleBase
    {
        SkywalkingOptions _options;
        public SkywalkingClientModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(SkywalkingOptions).Name).Get<SkywalkingOptions>();
            if (_options == null)
                _options = new SkywalkingOptions();
        }

        public override void DoHostBuild(IHostBuilder hostBuilder, IContainer container)
        {
            if (_options.Enable)
            {
                hostBuilder
                    .ConfigureServices(services => services.AddSingleton<ITracingDiagnosticProcessor, JimuClientDiagnosticProcessor>())
                    .AddSkyAPM();

            }
            base.DoHostBuild(hostBuilder, container);
        }

        public override void DoWebConfigureServices(IServiceCollection services, IContainer container)
        {
            if (_options.Enable)
            {
                services.AddSingleton<ITracingDiagnosticProcessor, JimuClientDiagnosticProcessor>();
            }

            base.DoWebConfigureServices(services, container);
        }
    }


}
