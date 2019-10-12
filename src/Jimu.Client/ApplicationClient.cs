namespace Jimu.Client
{
    public class ApplicationClient
    {
        public static void Run(string settingName = "JimuAppClientSettings")
        {
            new ApplicationClientBuilder(new Autofac.ContainerBuilder(), settingName).Build().Run();
        }
    }
}
