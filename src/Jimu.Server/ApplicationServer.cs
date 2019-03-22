using Jimu.Server;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server
{
    public class ApplicationServer
    {
        public static void Run(string settingName = "JimuAppServerSettings")
        {
            new ApplicationServerBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }
    }
}
