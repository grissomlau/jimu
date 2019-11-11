using Jimu.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Apm.Skywalking
{
    public class JimuServerDiagnosticModule : ServerModuleBase
    {
        public JimuServerDiagnosticModule(IConfigurationRoot jimuAppSettings) : base(jimuAppSettings)
        {
        }
    }
}
