using Autofac;
using Jimu.Module;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SkyApm;
using SkyApm.Agent.GeneralHost;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Diagnostic.Skywalking
{
    public class JimuServerDiagnosticModule : ServerGeneralModuleBase
    {
        SkywalkingOptions _options;
        public JimuServerDiagnosticModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(SkywalkingOptions).Name).Get<SkywalkingOptions>();
            if (_options == null)
                _options = new SkywalkingOptions();
        }

        public override void DoHostBuild(IHostBuilder hostBuilder)
        {
            if (_options.Enable)
            {
                hostBuilder
                    .ConfigureServices(services => services.AddSingleton<ITracingDiagnosticProcessor, JimuServerDiagnosticProcessor>())
                    .AddSkyAPM();

            }
            base.DoHostBuild(hostBuilder);
        }
    }
}
