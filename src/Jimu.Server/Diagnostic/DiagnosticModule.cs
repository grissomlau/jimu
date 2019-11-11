using Autofac;
using Jimu.Diagnostic;
using Jimu.Module;
using Microsoft.Extensions.Configuration;

namespace Jimu.Server.Diagnostic
{
    public class DiagnosticModule : ServerModuleBase
    {
        readonly DiagnosticServerOptions _options;
        public DiagnosticModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
            _options = jimuAppSettings.GetSection(typeof(DiagnosticServerOptions).Name).Get<DiagnosticServerOptions>();
            if (_options == null)
                _options = new DiagnosticServerOptions();
        }

        public override ModuleExecPriority Priority => ModuleExecPriority.Critical;

        public override void DoRegister(ContainerBuilder componentContainerBuilder)
        {
            componentContainerBuilder.RegisterInstance(new DiagnosticServer(_options)).As<IJimuDiagnostic>().SingleInstance();
            base.DoRegister(componentContainerBuilder);
        }
    }
}
