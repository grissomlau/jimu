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
