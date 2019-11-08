using Autofac;
using Jimu.APM;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.APM
{
    public class ApmModule : ClientModuleBase
    {
        readonly ApmClientOptions _options;
        public ApmModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(ApmClientOptions).Name).Get<ApmClientOptions>();
            if (_options == null)
                _options = new ApmClientOptions();
        }

        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            componentContainerBuilder.RegisterInstance(new ApmClient(_options)).As<IJimuApm>().SingleInstance();
            base.DoRegister(componentContainerBuilder);
        }
    }
}
