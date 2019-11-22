using Autofac;
using Jimu.Diagnostic;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Client.Diagnostic
{
    public class DiagnosticModule : ClientModuleBase
    {
        readonly DiagnosticClientOptions _options;
        public DiagnosticModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(DiagnosticClientOptions).Name).Get<DiagnosticClientOptions>();
            if (_options == null)
                _options = new DiagnosticClientOptions();
        }

        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            componentContainerBuilder.RegisterInstance(new DiagnosticClient(_options)).As<IJimuDiagnostic>().SingleInstance();
            base.DoRegister(componentContainerBuilder);
        }
    }
}
