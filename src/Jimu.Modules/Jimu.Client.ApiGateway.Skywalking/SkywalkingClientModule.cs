using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.Module;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyApm;

namespace Jimu.Client.ApiGateway.Skywalking
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

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            base.DoRegister(componentContainerBuilder);
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
