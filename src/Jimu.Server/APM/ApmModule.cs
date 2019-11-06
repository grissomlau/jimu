using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Jimu.APM;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.APM
{
    public class ApmModule : ServerModuleBase
    {
        readonly ApmServerOptions _options;
        public ApmModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(ApmServerOptions).Name).Get<ApmServerOptions>();
            if (_options == null)
                _options = new ApmServerOptions();
        }

        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            componentContainerBuilder.RegisterInstance(new ApmServer(_options)).As<IJimuApm>().SingleInstance();
            base.DoRegister(componentContainerBuilder);
        }
    }
}
